using System;
using BP.DA;
using BP.Web;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Admin_CCBPMDesigner_FlowDevModel : BP.WF.HttpHandler.DirectoryPageBase
    {
        #region 变量.
        /// <summary>
        /// 类别
        /// </summary>
        public string SortNo
        {
            get
            {
                return this.GetRequestVal("SortNo");
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetRequestVal("FlowName");
            }
        }
        public FlowDevModel FlowDevModel
        {
            get
            {
                return (FlowDevModel)this.GetRequestValInt("FlowDevModel");
            }
        }
        #endregion 变量.

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_CCBPMDesigner_FlowDevModel()
        {
        }
        /// <summary>
        /// 获取默认的开发模式.
        /// </summary>
        /// <returns></returns>
        public string Default_Init()
        {
            string sql = "SELECT val FROM Sys_GloVar WHERE No='FlowDevModel_" + BP.Web.WebUser.No + "'";
            int val = DBAccess.RunSQLReturnValInt(sql, 1);
            return val.ToString();
        }
        public string FlowDevModel_Save()
        {
            string SortNo = GetRequestVal("SortNo");
            string FlowName = GetRequestVal("FlowName");
            string url = GetRequestVal("Url");

            string FrmUrl = GetRequestVal("FrmUrl");
            string FrmID = GetRequestVal("FrmID");
            //执行创建流程模版.
            string flowNo = BP.WF.Template.TemplateGlo.NewFlow(SortNo, FlowName, DataStoreModel.ByCCFlow, null, null);
            Flow fl = new Flow(flowNo);
            fl.FlowDevModel = this.FlowDevModel; //流程开发模式.
            fl.HisDataStoreModel = DataStoreModel.SpecTable;
            fl.FrmUrl = FrmUrl;
            if (DataType.IsNullOrEmpty(FrmID) == true|| FrmID.Equals("undefined"))
                fl.FrmUrl = FrmUrl;
            else
                fl.FrmUrl = FrmID;
            fl.Update();

            //设置极简类型的表单信息.
            if (this.FlowDevModel == FlowDevModel.JiJian)
            {
                Nodes nds = new Nodes();
                nds.Retrieve(NodeAttr.FK_Flow, fl.No);
                foreach (Node nd in nds)
                {
                    nd.NodeFrmID = "ND" + int.Parse(fl.No) + "01";
                    if (nd.IsStartNode == false)
                    {
                        nd.FrmWorkCheckSta = FrmWorkCheckSta.Enable;

                        FrmNode fn = new FrmNode();
                        fn.FK_Frm = nd.NodeFrmID; 
                        fn.IsEnableFWC = FrmWorkCheckSta.Enable;
                        fn.FK_Node = nd.NodeID;
                        fn.FK_Flow = flowNo;
                        fn.FrmSln = FrmSln.Readonly;
                        fn.setMyPK(fn.FK_Frm + "_" + fn.FK_Node + "_" + fn.FK_Flow);
                        //执行保存.
                        fn.Save();
                    }
                    nd.DirectUpdate();
                }
            }

            //设置累加类型的表单信息.
            if (this.FlowDevModel == FlowDevModel.FoolTruck)
            {
                Nodes nds = new Nodes();
                nds.Retrieve(NodeAttr.FK_Flow, fl.No);
                foreach (Node nd in nds)
                {
                   //表单方案的保存
                    FrmNode fn = new FrmNode();
                    fn.FK_Frm = nd.NodeFrmID;
                    //fn.IsEnableFWC = FrmWorkCheckSta.Enable;
                    fn.FK_Node = nd.NodeID;
                    fn.FK_Flow = flowNo;
                    fn.FrmSln = FrmSln.Readonly;
                    fn.setMyPK(fn.FK_Frm + "_" + fn.FK_Node + "_" + fn.FK_Flow);
                    //执行保存.
                    fn.Save();
                    nd.HisFormType = NodeFormType.FoolTruck;
                    nd.DirectUpdate();
                   
                }
            }
            //设置绑定表单库的表单信息.
            if (this.FlowDevModel == FlowDevModel.RefOneFrmTree)
            {
                Nodes nds = new Nodes();
                nds.Retrieve(NodeAttr.FK_Flow, fl.No);
                foreach (Node nd in nds)
                {
                    nd.NodeFrmID = fl.FrmUrl;
                    if(nd.IsStartNode == true)
                        nd.FrmWorkCheckSta = FrmWorkCheckSta.Disable;
                    else
                        nd.FrmWorkCheckSta = FrmWorkCheckSta.Enable;
                    nd.HisFormType = NodeFormType.RefOneFrmTree;
                    nd.DirectUpdate();

                    FrmNode fn = new FrmNode();
                    fn.FK_Frm = nd.NodeFrmID;
                    if (nd.IsStartNode == true)
                    {
                        fn.IsEnableFWC = FrmWorkCheckSta.Disable;
                        fn.FrmSln = FrmSln.Default;
                    }
                       
                    else
                    {
                        fn.IsEnableFWC = FrmWorkCheckSta.Enable;
                        fn.FrmSln = FrmSln.Readonly;
                    }
                        
                    fn.FK_Node = nd.NodeID;
                    fn.FK_Flow = flowNo;
                    fn.FrmSln = FrmSln.Readonly;
                    fn.setMyPK(fn.FK_Frm + "_" + fn.FK_Node + "_" + fn.FK_Flow);
                    //执行保存.
                    fn.Save(); 
                }
            }

            if (this.FlowDevModel == FlowDevModel.SDKFrm)
            {
                Nodes nds = new Nodes();
                nds.Retrieve(NodeAttr.FK_Flow, fl.No);
                foreach (Node nd in nds)
                {
                    nd.HisFormType = NodeFormType.SDKForm;
                    nd.FormUrl = fl.FrmUrl;
                    nd.DirectUpdate();
                }
            }
            if (this.FlowDevModel == FlowDevModel.SelfFrm)
            {
                Nodes nds = new Nodes();
                nds.Retrieve(NodeAttr.FK_Flow, fl.No);
                foreach (Node nd in nds)
                {
                    nd.HisFormType = NodeFormType.SDKForm;
                    nd.FormUrl = fl.FrmUrl;
                    nd.DirectUpdate();
                }

            }

            ///保存模式.
            SaveModel(this.FlowDevModel);

            //返回流程编号
            return flowNo;
        }
        /// <summary>
        /// 保存模式
        /// </summary>
        /// <param name="val"></param>
        public void SaveModel(FlowDevModel val)
        {
            string pk = "FlowDevModel_" + WebUser.No;

            string sql = "SELECT Val FROM Sys_GloVar WHERE No='" + pk + "'";
            int valInt = DBAccess.RunSQLReturnValInt(sql, 1);
            if (valInt == (int)val)
                return;

            sql = "UPDATE Sys_GloVar SET Val=" + (int)val + " WHERE No='" + pk + "'";
            int myval = DBAccess.RunSQL(sql);
            if (myval == 1)
                return;

            sql = "INSERT INTO Sys_GloVar (No,Name,Val) VALUES('" + pk + "','FlowDevModel','" + (int)val + "')";
            DBAccess.RunSQL(sql);
        }
        /// <summary>
        /// 创建流程-早期版本模式
        /// </summary>
        /// <returns></returns>
        public string Default_NewFlowMode_0()
        {
            try
            {
                int runModel = this.GetRequestValInt("RunModel");
                string FlowName = this.GetRequestVal("FlowName");
                string FlowSort = this.GetRequestVal("FlowSort").Trim();
                FlowSort = FlowSort.Trim();

                int DataStoreModel = this.GetRequestValInt("DataStoreModel");
                string PTable = this.GetRequestVal("PTable");
                string FlowMark = this.GetRequestVal("FlowMark");
                int FlowFrmModel = this.GetRequestValInt("FlowFrmModel");
                string FrmUrl = this.GetRequestVal("FrmUrl");
                string FlowVersion = this.GetRequestVal("FlowVersion");

                string flowNo = BP.WF.Template.TemplateGlo.NewFlow(FlowSort, FlowName,
                        Template.DataStoreModel.SpecTable, PTable, FlowMark);

                Flow fl = new Flow(flowNo);


                //清空WF_Emp 的StartFlows ,让其重新计算.
                // DBAccess.RunSQL("UPDATE  WF_Emp Set StartFlows =''");
                return flowNo;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

    }
}
