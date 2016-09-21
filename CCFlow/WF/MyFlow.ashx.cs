using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using BP.WF;
using BP.Port;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;
using BP.WF.Template;
using BP.WF.Data;
using BP.Sys;

namespace CCFlow.WF
{
    /// <summary>
    /// MyFlow 的摘要说明
    /// </summary>
    public class MyFlow : IHttpHandler
    {
        HttpRequest Request;

        #region  运行变量
        /// <summary>
        /// 当前的流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                string s = this.Request.Form["FK_Flow"];
                if (string.IsNullOrEmpty(s))
                    throw new Exception("@流程编号参数错误...");

                return BP.WF.Dev2Interface.TurnFlowMarkToFlowNo(s);
            }
        }
        /// <summary>
        /// 从节点.
        /// </summary>
        public string FromNode
        {
            get
            {
                return this.Request.Form["FromNode"];
            }
        }
        /// <summary>
        /// 执行功能
        /// </summary>
        public string DoFunc
        {
            get
            {
                return this.Request.Form["DoFunc"];
            }
        }
        /// <summary>
        /// 子流程编号
        /// </summary>
        public string CFlowNo
        {
            get
            {
                return this.Request.Form["CFlowNo"];
            }
        }
        /// <summary>
        /// 工作IDs
        /// </summary>
        public string WorkIDs
        {
            get
            {
                return this.Request.Form["WorkIDs"];
            }
        }
        /// <summary>
        /// Nos
        /// </summary>
        public string Nos
        {
            get
            {
                return this.Request.Form["Nos"];
            }
        }
        /// <summary>
        /// 是否抄送
        /// </summary>
        public bool IsCC
        {
            get
            {

                if (string.IsNullOrEmpty(this.Request.Form["Paras"]) == false)
                {
                    string myps = this.Request.Form["Paras"];

                    if (myps.Contains("IsCC=1") == true)
                        return true;
                }
                if (string.IsNullOrEmpty(this.Request.Form["AtPara"]) == false)
                {
                    string myps = this.Request.Form["AtPara"];

                    if (myps.Contains("IsCC=1") == true)
                        return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 当前的工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {

                if (this.Request.Form["WorkID"] == null)
                    return 0;
                else
                    return Int64.Parse(this.Request.Form["WorkID"]);
            }
        }
        /// <summary>
        /// 子流程ID
        /// </summary>
        public Int64 CWorkID
        {
            get
            {
                
                    if (this.Request.Form["CWorkID"] == null)
                        return 0;
                    else
                        return Int64.Parse(this.Request.Form["CWorkID"]);
            }
        }
        private int _FK_Node = 0;
        /// <summary>
        /// 当前的 NodeID ,在开始时间,nodeID,是地一个,流程的开始节点ID.
        /// </summary>
        public int FK_Node
        {
            get
            {
                string fk_nodeReq = this.Request.Form["FK_Node"];
                if (string.IsNullOrEmpty(fk_nodeReq))
                    fk_nodeReq = this.Request.Form["NodeID"];

                if (string.IsNullOrEmpty(fk_nodeReq) == false)
                    return int.Parse(fk_nodeReq);

                if (_FK_Node == 0)
                {
                    if (this.Request.Form["WorkID"] != null)
                    {
                        string sql = "SELECT FK_Node from  WF_GenerWorkFlow where WorkID=" + this.WorkID;
                        _FK_Node = DBAccess.RunSQLReturnValInt(sql);
                    }
                    else
                    {
                        _FK_Node = int.Parse(this.FK_Flow + "01");
                    }
                }
                return _FK_Node;
            }
        }
        /// <summary>
        /// FID
        /// </summary>
        public int FID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.Form["FID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 父流程ID.
        /// </summary>
        public int PWorkID
        {
            get
            {
                try
                {
                    string s = this.Request.Form["PWorkID"];
                    if (string.IsNullOrEmpty(s) == true)
                        s = this.Request.Form["PWorkID"];
                    if (string.IsNullOrEmpty(s) == true)
                        s = "0";
                    return int.Parse(s);
                }
                catch
                {
                    return 0;
                }
            }
        }
        private string _width = "";
        /// <summary>
        /// 表单宽度
        /// </summary>
        public string Width
        {
            get
            {
                return _width;
                //float w = float.Parse(_width)+20;
                //return w.ToString();
            }
            set { _width = value; }
        }
        private string _height = "";
        /// <summary>
        /// 表单高度
        /// </summary>
        public string Height
        {
            get { return _height; }
            set { _height = value; }
        }
        public string _btnWord = "";
        public string BtnWord
        {
            get { return _btnWord; }
            set { _btnWord = value; }
        }
        #endregion


        public string InitToolBar()
        {
            string toolbar = "";
            toolbar += "<input type=button onclick='Send()' value='发送'/>";
            return toolbar;
        }
        public string Send()
        {
            //表单的值  KEY/VALUE
            string titleValue = Request.Form["Title"];
            return "发送成功.";
        }
        public string Save() {

            return "";
        }
        /// <summary>
        /// 产生一个工作节点
        /// </summary>
        /// <returns></returns>
        public string GenerWorkNode()
        {
            DataSet ds = BP.WF.CCFlowAPI.GenerWorkNode(this.FK_Flow, this.FK_Node, this.WorkID, 
                this.FID, BP.Web.WebUser.No);

            string xml = "c:\\WorkNode.xml";
            ds.WriteXml(xml);

            string json = BP.Tools.Json.ToJson(ds);
            BP.DA.DataType.WriteFile("c:\\WorkNode.json", json);

            return "";
        }
        public void ProcessRequest(HttpContext context)
        {
            this.Request = context.Request;

            context.Response.ContentType = "text/plain";
            string method = context.Request.QueryString["Method"].ToString();
            string resultValue = "";
            switch (method)
            {
                case "send":
                    resultValue = Send();
                    break;
                case "InitToolBar":
                    resultValue = InitToolBar();
                    break;
                case "GenerWorkNode":
                    resultValue = GenerWorkNode();
                    break;
                default:
                    resultValue = method + "没有";
                    break;
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(resultValue);
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