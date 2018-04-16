using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BP.Demo;
using BP.WF;
using BP.Demo.SDK;
using BP.Demo.FlowEvent;
using BP.Demo.BPFramework;
using BP.DA;

namespace CCFlow.SDKFlowDemo.SDK.F018
{
    /// <summary>
    /// Serv18 的摘要说明
    /// </summary>
    public class Serv18 : IHttpHandler
    {
        #region 参数.
        public void OutHtml(string msg)
        {
            //组装ajax字符串格式,返回调用客户端
            MyContext.Response.Charset = "UTF-8";
            MyContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
            MyContext.Response.ContentType = "text/html";
            MyContext.Response.Expires = 0;
            MyContext.Response.Write(msg);
            MyContext.Response.End();
        }
        /// <summary>
        /// 封装有关个别 HTTP 请求的所有 HTTP 特定的信息
        /// </summary>
        HttpContext MyContext = null;
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(MyContext.Request[param], System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 执行类型
        /// </summary>
        public string DoType
        {
            get
            {
                return getUTF8ToString("DoType");
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return getUTF8ToString("FK_Flow");
            }
        }
        /// <summary>
        /// 当前节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                string fk_node = getUTF8ToString("FK_Node");
                if (!DataType.IsNullOrEmpty(fk_node))
                    return Int32.Parse(getUTF8ToString("FK_Node"));
                return 0;
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(getUTF8ToString("WorkID"));
            }
        }
        /// <summary>
        /// 流程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return Int64.Parse(getUTF8ToString("FID"));
            }
        }
        #endregion 参数.

        public void ProcessRequest(HttpContext context)
        {
            MyContext = context;
            string result = "";
            switch (this.DoType)
            {
                case "Send":
                    result = this.Send();
                    break;
                default:
                    break;
            }

            //组装ajax字符串格式,返回调用客户端
            MyContext.Response.Charset = "UTF-8";
            MyContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
            MyContext.Response.ContentType = "text/html";
            MyContext.Response.Expires = 0;
            MyContext.Response.Write(result);
            MyContext.Response.End();
        }
        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {
            ND018Rpt rpt = null;
            //判断是否有父流程
            if (FID == 0)
                rpt = new ND018Rpt(WorkID);
            else
                rpt = new ND018Rpt(FID);

            //从内存中copy
            rpt = BP.Sys.PubClass.CopyFromRequest(rpt, MyContext.Request) as ND018Rpt;
            rpt.Update();
        }
        /// <summary>
        /// 发送
        /// </summary>
        /// <returns></returns>
        public string Send()
        {
            try
            {
                //获取发送成功后信息
                SendReturnObjs objs = null;
                ///执行保存.
                this.Save();

                //获取集合
                ND018Rpt rpt = null;
                if (FID == 0)
                    rpt = new ND018Rpt(WorkID);
                else
                    rpt = new ND018Rpt(FID);

                //填写请假申请表
                if (this.FK_Node == 1801)
                {
                    //发送到部门经理审批环节
                    objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID);

                    Node nd = new Node(this.FK_Node);
                    //写入日志
                    BP.WF.Dev2Interface.WriteTrack(FK_Flow,FK_Node,nd.Name, WorkID,FID,null,ActionType.WorkCheck,"",null,"审核");
                }
                //部门经理审批
                else if (this.FK_Node == 1802)
                {
                    //如果请假天数大于等于10天，发送到总经理审批环节
                    if(rpt.QingJiaTianShu>=10)
                        objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID,1803,null);
                    //如果不到10天，则直接发送到人力资源备案
                    else
                        objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID, 1899, null);
                    //将部门经理审批意见写入日志
                    Node nd = new Node(this.FK_Node);
                    BP.WF.Dev2Interface.WriteTrack(FK_Flow, FK_Node, nd.Name, WorkID, FID, rpt.NoteBM, ActionType.WorkCheck, "", null, "审核");
                }
                //总经理审批
                else if (this.FK_Node == 1803)
                {
                    //发动到人力资源部
                    objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow,this.WorkID);
                    //将总经理审批意见写入日志
                    Node nd = new Node(this.FK_Node);
                    BP.WF.Dev2Interface.WriteTrack(FK_Flow, FK_Node, nd.Name, WorkID, FID, rpt.NoteZJL, ActionType.WorkCheck, "", null, "审核");
                }
                /**
                 * 一下两步可以合成一步，
                 * 具体看流程图的设计，需要和流程图设计一样，方便理解
                 * **/
                //超过10天时，人力资源审批
                else if (this.FK_Node == 1804)
                {
                    //归档
                    objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow,this.WorkID);
                    //写入日志
                    Node nd = new Node(this.FK_Node);
                    BP.WF.Dev2Interface.WriteTrack(FK_Flow, FK_Node, nd.Name, WorkID, FID, rpt.NoteRL, ActionType.WorkCheck, "", null, "审核");
                }
                //不超过10天时，人力资源审批
                else if (this.FK_Node == 1899)
                {
                    //归档
                    objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID);
                    //写入日志
                    Node nd = new Node(this.FK_Node);
                    BP.WF.Dev2Interface.WriteTrack(FK_Flow, FK_Node, nd.Name, WorkID, FID, rpt.NoteRL, ActionType.WorkCheck, "", null, "审核");
                }

                //设置标题
                BP.WF.Dev2Interface.Flow_SetFlowTitle(FK_Flow.ToString(), WorkID, rpt.Title);
                //法功成功后提示信息
                string infor = objs.ToMsgOfHtml().Replace("@", "<br />");
                return infor;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}