using System;
using System.Collections.Generic;
using System.Collections;
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
using LitJson;
using BP.WF.XML;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 初始化函数
    /// </summary>
    public class WF_Admin_CCBPMDesigner2018 : DirectoryPageBase
    {
        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin_CCBPMDesigner2018(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {

            return "err@没有判断的标记:" + this.DoType;
        }
        #endregion 执行父类的重写方法.
          

         

        #region 节点相关 Nodes
        /// <summary>
        /// 创建流程节点并返回编号
        /// </summary>
        /// <returns></returns>
        public string CreateNode()
        {
            try
            {
                string FK_Flow = this.GetRequestVal("FK_Flow");
                string x = this.GetRequestVal("x");
                string y = this.GetRequestVal("y");
                int iX = 20;
                int iY = 20;
                
                if (DataType.IsNullOrEmpty(x)==false) 
                    iX = (int)double.Parse(x);

                if (DataType.IsNullOrEmpty(y)==false) 
                    iY = (int)double.Parse(y);

                int nodeId = BP.WF.Template.TemplateGlo.NewNode(FK_Flow, iX, iY);

                BP.WF.Node node = new BP.WF.Node(nodeId);
                node.Update();

                Hashtable ht = new Hashtable();
                ht.Add("NodeID", node.NodeID);
                ht.Add("Name", node.Name);

                return BP.Tools.Json.ToJsonEntityModel(ht);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 根据节点编号删除流程节点
        /// </summary>
        /// <returns>执行结果</returns>
        public string DeleteNode()
        {
            try
            {
                BP.WF.Node node = new BP.WF.Node();
                node.NodeID = this.FK_Node;
                if (node.RetrieveFromDBSources() == 0)
                    return "err@删除失败,没有删除到数据，估计该节点已经别删除了.";

                if (node.IsStartNode == true)
                    return "err@开始节点不允许被删除。";

                node.Delete();
                return "删除成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 修改节点名称
        /// </summary>
        /// <returns></returns>
        public string Node_EditNodeName()
        {
            string FK_Node = this.GetValFromFrmByKey("NodeID");
            string NodeName = System.Web.HttpContext.Current.Server.UrlDecode(this.GetValFromFrmByKey("NodeName"));

            BP.WF.Node node = new BP.WF.Node();
            node.NodeID = int.Parse(FK_Node);
            int iResult = node.RetrieveFromDBSources();
            if (iResult > 0)
            {
                node.Name = NodeName;
                node.Update();
                return "@修改成功.";
            }

            return "err@修改节点失败，请确认该节点是否存在？";
        }
        /// <summary>
        /// 修改节点运行模式
        /// </summary>
        /// <returns></returns>
        public string Node_ChangeRunModel()
        {
            string runModel = GetValFromFrmByKey("RunModel");
            BP.WF.Node node = new BP.WF.Node(this.FK_Node);
            //节点运行模式
            switch (runModel)
            {
                case "NodeOrdinary":
                    node.HisRunModel = BP.WF.RunModel.Ordinary;
                    break;
                case "NodeFL":
                    node.HisRunModel = BP.WF.RunModel.FL;
                    break;
                case "NodeHL":
                    node.HisRunModel = BP.WF.RunModel.HL;
                    break;
                case "NodeFHL":
                    node.HisRunModel = BP.WF.RunModel.FHL;
                    break;
                case "NodeSubThread":
                    node.HisRunModel = BP.WF.RunModel.SubThread;
                    break;
            }
            node.Update();

            return "设置成功.";
        }
        #endregion end Node

    }
}
