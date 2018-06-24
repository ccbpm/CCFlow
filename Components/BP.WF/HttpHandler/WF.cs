﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.WF;
using BP.WF.Rpt;
using BP.WF.Data;
using BP.WF.Template;
using BP.WF.Port;
using BP.Web;

namespace BP.WF.HttpHandler
{
    public class WF : DirectoryPageBase
    {
        #region 单表单查看.
        /// <summary>
        /// 流程单表单查看
        /// </summary>
        /// <returns></returns>
        public string FrmView_Init()
        {
            Node nd = new Node(this.FK_Node);
            nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.

            MapData md = new MapData();
            md.No = nd.NodeFrmID;
            if (md.RetrieveFromDBSources() == 0)
                throw new Exception("装载错误，该表单ID=" + md.No + "丢失，请修复一次流程重新加载一次.");

           

            //获得表单模版.
            DataSet myds = BP.Sys.CCFormAPI.GenerHisDataSet(md.No);


            #region 把主从表数据放入里面.
            //.工作数据放里面去, 放进去前执行一次装载前填充事件.
            BP.WF.Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.RetrieveFromDBSources();

            //重设默认值.
            wk.ResetDefaultVal();

            DataTable mainTable = wk.ToDataTableField("MainTable");
            mainTable.TableName = "MainTable";
            myds.Tables.Add(mainTable);
            #endregion


            //加入WF_Node.
            DataTable WF_Node = nd.ToDataTableField("WF_Node");
            myds.Tables.Add(WF_Node);
             

            #region 加入组件的状态信息, 在解析表单的时候使用.
            nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.
            BP.WF.Template.FrmNodeComponent fnc = new FrmNodeComponent(nd.NodeID);
            if (nd.NodeFrmID != "ND" + nd.NodeID)
            {
                /*说明这是引用到了其他节点的表单，就需要把一些位置元素修改掉.*/
                int refNodeID = int.Parse(nd.NodeFrmID.Replace("ND", ""));

                BP.WF.Template.FrmNodeComponent refFnc = new FrmNodeComponent(refNodeID);

                fnc.SetValByKey(FrmWorkCheckAttr.FWC_H, refFnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_H));
                fnc.SetValByKey(FrmWorkCheckAttr.FWC_W, refFnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_W));
                fnc.SetValByKey(FrmWorkCheckAttr.FWC_X, refFnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_X));
                fnc.SetValByKey(FrmWorkCheckAttr.FWC_Y, refFnc.GetValFloatByKey(FrmWorkCheckAttr.FWC_Y));


                fnc.SetValByKey(FrmSubFlowAttr.SF_H, refFnc.GetValFloatByKey(FrmSubFlowAttr.SF_H));
                fnc.SetValByKey(FrmSubFlowAttr.SF_W, refFnc.GetValFloatByKey(FrmSubFlowAttr.SF_W));
                fnc.SetValByKey(FrmSubFlowAttr.SF_X, refFnc.GetValFloatByKey(FrmSubFlowAttr.SF_X));
                fnc.SetValByKey(FrmSubFlowAttr.SF_Y, refFnc.GetValFloatByKey(FrmSubFlowAttr.SF_Y));

                fnc.SetValByKey(FrmThreadAttr.FrmThread_H, refFnc.GetValFloatByKey(FrmThreadAttr.FrmThread_H));
                fnc.SetValByKey(FrmThreadAttr.FrmThread_W, refFnc.GetValFloatByKey(FrmThreadAttr.FrmThread_W));
                fnc.SetValByKey(FrmThreadAttr.FrmThread_X, refFnc.GetValFloatByKey(FrmThreadAttr.FrmThread_X));
                fnc.SetValByKey(FrmThreadAttr.FrmThread_Y, refFnc.GetValFloatByKey(FrmThreadAttr.FrmThread_Y));

                fnc.SetValByKey(FrmTrackAttr.FrmTrack_H, refFnc.GetValFloatByKey(FrmTrackAttr.FrmTrack_H));
                fnc.SetValByKey(FrmTrackAttr.FrmTrack_W, refFnc.GetValFloatByKey(FrmTrackAttr.FrmTrack_W));
                fnc.SetValByKey(FrmTrackAttr.FrmTrack_X, refFnc.GetValFloatByKey(FrmTrackAttr.FrmTrack_X));
                fnc.SetValByKey(FrmTrackAttr.FrmTrack_Y, refFnc.GetValFloatByKey(FrmTrackAttr.FrmTrack_Y));

                fnc.SetValByKey(FTCAttr.FTC_H, refFnc.GetValFloatByKey(FTCAttr.FTC_H));
                fnc.SetValByKey(FTCAttr.FTC_W, refFnc.GetValFloatByKey(FTCAttr.FTC_W));
                fnc.SetValByKey(FTCAttr.FTC_X, refFnc.GetValFloatByKey(FTCAttr.FTC_X));
                fnc.SetValByKey(FTCAttr.FTC_Y, refFnc.GetValFloatByKey(FTCAttr.FTC_Y));
            }

            myds.Tables.Add(fnc.ToDataTableField("WF_FrmNodeComponent"));
            #endregion 加入组件的状态信息, 在解析表单的时候使用.

            #region 增加附件信息.
            BP.Sys.FrmAttachments athDescs = new FrmAttachments();

            nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.
            athDescs.Retrieve(FrmAttachmentAttr.FK_MapData, nd.NodeFrmID);
            if (athDescs.Count != 0)
            {
                FrmAttachment athDesc = athDescs[0] as FrmAttachment;

                //查询出来数据实体.
                BP.Sys.FrmAttachmentDBs dbs = new BP.Sys.FrmAttachmentDBs();
                if (athDesc.HisCtrlWay == AthCtrlWay.PWorkID)
                {
                    string pWorkID = BP.DA.DBAccess.RunSQLReturnValInt("SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + this.WorkID, 0).ToString();
                    if (pWorkID == null || pWorkID == "0")
                        pWorkID = this.WorkID.ToString();

                    if (athDesc.AthUploadWay == AthUploadWay.Inherit)
                    {
                        /* 继承模式 */
                        BP.En.QueryObject qo = new BP.En.QueryObject(dbs);
                        qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, pWorkID);
                        qo.addOr();
                        qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, this.WorkID);
                        qo.addOrderBy("RDT");
                        qo.DoQuery();
                    }

                    if (athDesc.AthUploadWay == AthUploadWay.Interwork)
                    {
                        /*共享模式*/
                        dbs.Retrieve(FrmAttachmentDBAttr.RefPKVal, pWorkID);
                    }
                }
                else if (athDesc.HisCtrlWay == AthCtrlWay.WorkID)
                {
                    /* 继承模式 */
                    BP.En.QueryObject qo = new BP.En.QueryObject(dbs);
                    qo.AddWhere(FrmAttachmentDBAttr.NoOfObj, athDesc.NoOfObj);
                    qo.addAnd();
                    qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, this.WorkID);
                    qo.addOrderBy("RDT");
                    qo.DoQuery();
                }

                //增加一个数据源.
                myds.Tables.Add(dbs.ToDataTableField("Sys_FrmAttachmentDB"));
            }
            #endregion

            #region 把外键表加入DataSet
            DataTable dtMapAttr = myds.Tables["Sys_MapAttr"];

            MapExts mes = md.MapExts;
            MapExt me = new MapExt();
            DataTable dt = new DataTable();
            foreach (DataRow dr in dtMapAttr.Rows)
            {
                string lgType = dr["LGType"].ToString();
                if (lgType.Equals("2")==false)
                    continue;

                string UIIsEnable = dr["UIVisible"].ToString();
                if (UIIsEnable == "0")
                    continue;

                string uiBindKey = dr["UIBindKey"].ToString();
                if (DataType.IsNullOrEmpty(uiBindKey) == true)
                {
                    string myPK = dr["MyPK"].ToString();
                    /*如果是空的*/
                    //   throw new Exception("@属性字段数据不完整，流程:" + fl.No + fl.Name + ",节点:" + nd.NodeID + nd.Name + ",属性:" + myPK + ",的UIBindKey IsNull ");
                }

                // 检查是否有下拉框自动填充。
                string keyOfEn = dr["KeyOfEn"].ToString();
                string fk_mapData = dr["FK_MapData"].ToString();

                #region 处理下拉框数据范围. for 小杨.
                me = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.AutoFullDLL, MapExtAttr.AttrOfOper, keyOfEn) as MapExt;
                if (me != null)
                {
                    string fullSQL = me.Doc.Clone() as string;
                    fullSQL = fullSQL.Replace("~", ",");
                    fullSQL = BP.WF.Glo.DealExp(fullSQL, wk, null);
                    dt = DBAccess.RunSQLReturnTable(fullSQL);
                    //重构新表
                    DataTable dt_FK_Dll = new DataTable();
                    dt_FK_Dll.TableName = keyOfEn;//可能存在隐患，如果多个字段，绑定同一个表，就存在这样的问题.
                    dt_FK_Dll.Columns.Add("No", typeof(string));
                    dt_FK_Dll.Columns.Add("Name", typeof(string));
                    foreach (DataRow dllRow in dt.Rows)
                    {
                        DataRow drDll = dt_FK_Dll.NewRow();
                        drDll["No"] = dllRow["No"];
                        drDll["Name"] = dllRow["Name"];
                        dt_FK_Dll.Rows.Add(drDll);
                    }
                    myds.Tables.Add(dt_FK_Dll);
                    continue;
                }
                #endregion 处理下拉框数据范围.

                // 判断是否存在.
                if (myds.Tables.Contains(uiBindKey) == true)
                    continue;

                myds.Tables.Add(BP.Sys.PubClass.GetDataTableByUIBineKey(uiBindKey));
            }
            #endregion End把外键表加入DataSet

            #region 图片附件
            nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.
            FrmImgAthDBs imgAthDBs = new FrmImgAthDBs(nd.NodeFrmID, this.WorkID.ToString());
            if (imgAthDBs != null && imgAthDBs.Count > 0)
            {
                DataTable dt_ImgAth = imgAthDBs.ToDataTableField("Sys_FrmImgAthDB");
                myds.Tables.Add(dt_ImgAth);
            }
            #endregion


            return BP.Tools.Json.ToJson(myds);
        }
        #endregion

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="mycontext"></param>
        public WF(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        protected override string DoDefaultMethod()
        {
            return base.DoDefaultMethod();
        }

        #region 我的关注流程.
        /// <summary>
        /// 我的关注流程
        /// </summary>
        /// <returns></returns>
        public string Focus_Init()
        {
            string flowNo = this.GetRequestVal("FK_Flow");

            int idx = 0;
            //获得关注的数据.
            System.Data.DataTable dt = BP.WF.Dev2Interface.DB_Focus(flowNo, BP.Web.WebUser.No);
            SysEnums stas = new SysEnums("WFSta");
            string[] tempArr;
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                int wfsta = int.Parse(dr["WFSta"].ToString());
                //edit by liuxc,2016-10-22,修复状态显示不正确问题
                string wfstaT = (stas.GetEntityByKey(SysEnumAttr.IntKey, wfsta) as SysEnum).Lab;
                string currEmp = string.Empty;

                if (wfsta != (int)BP.WF.WFSta.Complete)
                {
                    //edit by liuxc,2016-10-24,未完成时，处理当前处理人，只显示处理人姓名
                    foreach (string emp in dr["ToDoEmps"].ToString().Split(';'))
                    {
                        tempArr = emp.Split(',');

                        currEmp += tempArr.Length > 1 ? tempArr[1] : tempArr[0] + ",";
                    }

                    currEmp = currEmp.TrimEnd(',');

                    //currEmp = dr["ToDoEmps"].ToString();
                    //currEmp = currEmp.TrimEnd(';');
                }
                dr["ToDoEmps"] = currEmp;
                dr["FlowNote"] = wfstaT;
                dr["AtPara"] = (wfsta == (int)BP.WF.WFSta.Complete ? dr["Sender"].ToString().TrimStart('(').TrimEnd(')').Split(',')[1] : "");
            }
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 取消关注
        /// </summary>
        /// <returns></returns>
        public string Focus_Delete()
        {
            BP.WF.Dev2Interface.Flow_Focus(this.WorkID);
            return "执行成功";
        }
        #endregion 我的关注.

        /// <summary>
        /// 方法
        /// </summary>
        /// <returns></returns>
        public string HandlerMapExt()
        {
            WF_CCForm wf = new WF_CCForm(context);
            return wf.HandlerMapExt();
        }
        /// <summary>
        /// 节水公司
        /// </summary>
        /// <returns></returns>
        public string Start_InitTianYe_JieShui()
        {
            //获得当前人员的部门,根据部门获得该人员的组织集合.
            Paras ps = new Paras();
            ps.SQL = "SELECT FK_Dept FROM Port_DeptEmp WHERE FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";
            ps.AddFK_Emp();
            DataTable dt = DBAccess.RunSQLReturnTable(ps);

            //找到当前人员所在的部门集合, 应该找到他的组织集合为了减少业务逻辑.
            string orgNos = "'18099','103'"; //空的数据.
            foreach (DataRow dr in dt.Rows)
            {
                string deptNo = dr[0].ToString();
                orgNos += ",'" + deptNo + "'";
            }

            #region 获取类别列表(根据当前人员所在组织结构进行过滤类别.)
            FlowSorts fss = new FlowSorts();
            BP.En.QueryObject qo = new En.QueryObject(fss);
            qo.AddWhereIn(FlowSortAttr.OrgNo, "(" + orgNos + ")");  //指定的类别.
            qo.addOr();
            qo.AddWhere(FlowSortAttr.Name, " LIKE ", "%节水%");  //指定的类别.

            //排序.
            qo.addOrderBy(FlowSortAttr.No, FlowSortAttr.Idx);

            DataTable dtSort = qo.DoQueryToTable();
            dtSort.TableName = "Sort";
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dtSort.Columns["NO"].ColumnName = "No";
                dtSort.Columns["NAME"].ColumnName = "Name";
                dtSort.Columns["PARENTNO"].ColumnName = "ParentNo";
                dtSort.Columns["ORGNO"].ColumnName = "OrgNo";
            }

            //定义容器.
            DataSet ds = new DataSet();
            ds.Tables.Add(dtSort); //增加到里面去.
            #endregion 获取类别列表.

            //构造流程实例数据容器。
            DataTable dtStart = new DataTable();
            dtStart.TableName = "Start";
            dtStart.Columns.Add("No");
            dtStart.Columns.Add("Name");
            dtStart.Columns.Add("FK_FlowSort");
            dtStart.Columns.Add("IsBatchStart");
            dtStart.Columns.Add("IsStartInMobile");
            dtStart.Columns.Add("Note");

            //获得所有的流程（包含了所有子公司与集团的可以发起的流程但是没有根据组织结构进行过滤.）
            DataTable dtAllFlows = Dev2Interface.DB_StarFlows(Web.WebUser.No);

            //按照当前用户的流程类别权限进行过滤.
            foreach (DataRow drSort in dtSort.Rows)
            {
                foreach (DataRow drFlow in dtAllFlows.Rows)
                {
                    if (drSort["No"].ToString() != drFlow["FK_FlowSort"].ToString())
                        continue;

                    DataRow drNew = dtStart.NewRow();

                    drNew["No"] = drFlow["No"];
                    drNew["Name"] = drFlow["Name"];
                    drNew["FK_FlowSort"] = drFlow["FK_FlowSort"];
                    drNew["IsBatchStart"] = drFlow["IsBatchStart"];
                    drNew["IsStartInMobile"] = drFlow["IsStartInMobile"];
                    drNew["Note"] = drFlow["Note"];
                    dtStart.Rows.Add(drNew); //增加到里里面去.
                }
            }

            //把经过权限过滤的流程实体放入到集合里.
            ds.Tables.Add(dtStart); //增加到里面去.

            //返回组合
            string json = BP.Tools.Json.DataSetToJson(ds, false);

            //放入缓存里面去.
            BP.WF.Port.WFEmp em = new WFEmp();
            em.No = BP.Web.WebUser.No;

            //把json存入数据表，避免下一次再取.
            if (json.Length > 40)
            {
                em.StartFlows = json;
                em.Update();
            }
            return json;
        }
        /// <summary>
        /// 天业集团的发起，特殊处理.
        /// </summary>
        /// <returns></returns>
        public string Start_InitTianYe()
        {
            //如果请求了刷新.
            if (this.GetRequestVal("IsRef") != null)
            {
                //清除权限.
                DBAccess.RunSQL("UPDATE WF_Emp SET StartFlows='' WHERE No='" + BP.Web.WebUser.No + "' ");

                //处理权限,为了防止未知的错误.
                DBAccess.RunSQL("UPDATE WF_FLOWSORT SET ORGNO='0' WHERE ORGNO='' OR ORGNO IS NULL OR ORGNO='101'");

                DBAccess.RunSQL("UPDATE wf_flowsort SET ORGNO = REPLACE(NO,'Inc','') where  no like 'Inc%'");
            }

            //需要翻译.
            BP.WF.Port.WFEmp em = new WFEmp();
            em.No = BP.Web.WebUser.No;
            if (em.RetrieveFromDBSources() == 0)
            {
                em.FK_Dept = BP.Web.WebUser.FK_Dept;
                em.Name = Web.WebUser.Name;
                em.Insert();
            }
            string json = em.StartFlows;
            if (DataType.IsNullOrEmpty(json) == false)
                return json;

            //如果是节水公司的，就特别处理.
            if (WebUser.FK_Dept.IndexOf("18099") == 0)
                return Start_InitTianYe_JieShui();

            //获得当前人员的部门,根据部门获得该人员的组织集合.
            Paras ps = new Paras();
            ps.SQL = "SELECT FK_Dept FROM Port_DeptEmp WHERE FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";
            ps.AddFK_Emp();
            DataTable dt = DBAccess.RunSQLReturnTable(ps);

            //找到当前人员所在的部门集合, 应该找到他的组织集合为了减少业务逻辑.
            string orgNos = "'0'";
            foreach (DataRow dr in dt.Rows)
            {
                string deptNo = dr[0].ToString();
                orgNos += ",'" + deptNo + "'";
            }

            #region 获取类别列表(根据当前人员所在组织结构进行过滤类别.)
            FlowSorts fss = new FlowSorts();
            BP.En.QueryObject qo = new En.QueryObject(fss);
            if (orgNos.Contains(",") == false)
            {
                qo.AddWhere(FlowSortAttr.OrgNo, "0");  //..
                qo.addOr();
                qo.AddWhere(FlowSortAttr.OrgNo, "");  //..
            }
            else
            {
                qo.AddWhereIn(FlowSortAttr.OrgNo, "(" + orgNos + ")");  //指定的类别.
            }

            //排序.
            qo.addOrderBy(FlowSortAttr.No, FlowSortAttr.Idx);

            DataTable dtSort = qo.DoQueryToTable();
            dtSort.TableName = "Sort";
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dtSort.Columns["NO"].ColumnName = "No";
                dtSort.Columns["NAME"].ColumnName = "Name";
                dtSort.Columns["PARENTNO"].ColumnName = "ParentNo";
                dtSort.Columns["ORGNO"].ColumnName = "OrgNo";
            }

            //定义容器.
            DataSet ds = new DataSet();
            ds.Tables.Add(dtSort); //增加到里面去.
            #endregion 获取类别列表.

            //构造流程实例数据容器。
            DataTable dtStart = new DataTable();
            dtStart.TableName = "Start";
            dtStart.Columns.Add("No");
            dtStart.Columns.Add("Name");
            dtStart.Columns.Add("FK_FlowSort");
            dtStart.Columns.Add("IsBatchStart");
            dtStart.Columns.Add("IsStartInMobile");
            dtStart.Columns.Add("Note");

            //获得所有的流程（包含了所有子公司与集团的可以发起的流程但是没有根据组织结构进行过滤.）
            DataTable dtAllFlows = Dev2Interface.DB_StarFlows(Web.WebUser.No);

            //按照当前用户的流程类别权限进行过滤.
            foreach (DataRow drSort in dtSort.Rows)
            {
                foreach (DataRow drFlow in dtAllFlows.Rows)
                {
                    if (drSort["No"].ToString() != drFlow["FK_FlowSort"].ToString())
                        continue;

                    DataRow drNew = dtStart.NewRow();

                    drNew["No"] = drFlow["No"];
                    drNew["Name"] = drFlow["Name"];
                    drNew["FK_FlowSort"] = drFlow["FK_FlowSort"];
                    drNew["IsBatchStart"] = drFlow["IsBatchStart"];
                    drNew["IsStartInMobile"] = drFlow["IsStartInMobile"];
                    drNew["Note"] = drFlow["Note"];
                    dtStart.Rows.Add(drNew); //增加到里里面去.
                }
            }

            //把经过权限过滤的流程实体放入到集合里.
            ds.Tables.Add(dtStart); //增加到里面去.

            //返回组合
            json = BP.Tools.Json.DataSetToJson(ds, false);

            //把json存入数据表，避免下一次再取.
            if (json.Length > 40)
            {
                em.StartFlows = json;
                em.Update();
            }

            return json;
        }
        /// <summary>
        /// 获得发起列表 
        /// </summary>
        /// <returns></returns>
        public string Start_Init()
        {
            //通用的处理器.
            if (BP.Sys.SystemConfig.CustomerNo == "TianYe")
                return Start_InitTianYe();

            //定义容器.
            DataSet ds = new DataSet();

            //流程类别.
            FlowSorts fss = new FlowSorts();
            fss.RetrieveAll();

            DataTable dtSort = fss.ToDataTableField("Sort");
            dtSort.TableName = "Sort";
            ds.Tables.Add(dtSort);

            //获得能否发起的流程.
            DataTable dtStart = Dev2Interface.DB_StarFlows(Web.WebUser.No);
            dtStart.TableName = "Start";
            ds.Tables.Add(dtStart);

            //返回组合
            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        /// <summary>
        /// 获得发起列表
        /// </summary>
        /// <returns></returns>
        public string FlowSearch_Init()
        {
            DataSet ds = new DataSet();

            //流程类别.
            FlowSorts fss = new FlowSorts();
            fss.RetrieveAll();

            DataTable dtSort = fss.ToDataTableField("Sort");
            dtSort.TableName = "Sort";
            ds.Tables.Add(dtSort);

            //获得能否发起的流程.
            DataTable dtStart = DBAccess.RunSQLReturnTable("SELECT No,Name, FK_FlowSort FROM WF_Flow ORDER BY FK_FlowSort,Idx");
            dtStart.TableName = "Start";

            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dtStart.Columns["NO"].ColumnName = "No";
                dtStart.Columns["NAME"].ColumnName = "Name";
                dtStart.Columns["FK_FLOWSORT"].ColumnName = "FK_FlowSort";
            }

            ds.Tables.Add(dtStart);



            //返回组合
            return BP.Tools.Json.DataSetToJson(ds,false);
        }
        #region 获得列表.
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="UserNo">人员编号</param>
        /// <param name="fk_flow">流程编号</param>
        /// <returns>运行中的流程</returns>
        public string Runing_Init()
        {
            WF_App_ACE page = new WF_App_ACE(context);
            return page.Runing_Init();
        }
        /// <summary>
        /// 执行撤销
        /// </summary>
        /// <returns></returns>
        public string Runing_UnSend()
        {
            try
            {
                return BP.WF.Dev2Interface.Flow_DoUnSend(this.FK_Flow, this.WorkID);
            }
            catch(Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 执行催办
        /// </summary>
        /// <returns></returns>
        public string Runing_Press()
        {
            try
            {
                return BP.WF.Dev2Interface.Flow_DoPress(this.WorkID, this.GetRequestVal("Msg"),false);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 打开表单
        /// </summary>
        /// <returns></returns>
        public string Runing_OpenFrm()
        {
            int nodeID = this.FK_Node;
            GenerWorkFlow gwf = null;
            if (nodeID == 0)
            {
                gwf = new GenerWorkFlow(this.WorkID);
                nodeID = gwf.FK_Node;
            }

            string appPath = BP.WF.Glo.CCFlowAppPath;
            Node nd = null;
            Track tk = new Track();
            tk.FK_Flow = this.FK_Flow;


            tk.WorkID = this.WorkID;
            if (this.MyPK != null)
            {
                tk = new Track(this.FK_Flow, this.MyPK);
                nd = new Node(tk.NDFrom);
            }
            else
            {
                nd = new Node(nodeID);
            }

            Flow fl = new Flow(this.FK_Flow);
            Int64 workid = 0;
            if (nd.HisRunModel == RunModel.SubThread)
            {
                if (tk.FID == 0)
                {
                    if (gwf == null)
                        gwf = new GenerWorkFlow(this.WorkID);

                    workid = gwf.FID;
                }
                else
                {
                    workid = tk.FID;
                }
            }
            else
                workid = tk.WorkID;

            Int64 fid = this.FID;
            if (this.FID == 0)
                fid = tk.FID;

            if (fid > 0)
                workid = fid;

            if (workid == 0)
                workid = this.WorkID;

            string urlExt = "";
            DataTable ndrpt = DBAccess.RunSQLReturnTable("SELECT PFlowNo,PWorkID FROM " + fl.PTable + " WHERE OID=" + workid);
            if (ndrpt.Rows.Count == 0)
                urlExt = "&PFlowNo=0&PWorkID=0&IsToobar=0&IsHidden=true";
            else
                urlExt = "&PFlowNo=" + ndrpt.Rows[0]["PFlowNo"] + "&PWorkID=" + ndrpt.Rows[0]["PWorkID"] + "&IsToobar=0&IsHidden=true";
            urlExt += "&From=CCFlow&TruckKey=" + tk.GetValStrByKey("MyPK") + "&DoType=" + this.DoType + "&UserNo=" + WebUser.No ?? string.Empty + "&SID=" + WebUser.SID ?? string.Empty;

            urlExt = urlExt.Replace("PFlowNo=null", "");
            urlExt = urlExt.Replace("PWorkID=null", "");

            if (nd.HisFormType == NodeFormType.SDKForm || nd.HisFormType == NodeFormType.SelfForm)
            {
                //added by liuxc,2016-01-25
                if (nd.FormUrl.Contains("?"))
                    return "url@" + nd.FormUrl + "&IsReadonly=1&WorkID=" + workid + "&FK_Node=" + nd.NodeID + "&FK_Flow=" + nd.FK_Flow + "&FID=" + fid + urlExt;

                return "url@" + nd.FormUrl + "?IsReadonly=1&WorkID=" + workid + "&FK_Node=" + nd.NodeID + "&FK_Flow=" + nd.FK_Flow + "&FID=" + fid + urlExt;
            }

            Work wk = nd.HisWork;
            wk.OID = workid;
            if (wk.RetrieveFromDBSources() == 0)
            {
                GERpt rtp = nd.HisFlow.HisGERpt;
                rtp.OID = workid;
                if (rtp.RetrieveFromDBSources() == 0)
                {
                    string info = "打开(" + nd.Name + ")错误";
                    info += "当前的节点数据已经被删除！！！<br> 造成此问题出现的原因如下。";
                    info += "1、当前节点数据被非法删除。";
                    info += "2、节点数据是退回人与被退回人中间的节点，这部分节点数据查看不支持。";
                    info += "技术信息:表" + wk.EnMap.PhysicsTable + " WorkID=" + workid;
                    return "err@" + info;
                }
                wk.Row = rtp.Row;
            }

            if (nd.HisFlow.IsMD5 && wk.IsPassCheckMD5() == false)
            {
                string err = "打开(" + nd.Name + ")错误";
                err += "当前的节点数据已经被篡改，请报告管理员。";
                return "err@" + err;
            }

            nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.

            if (nd.HisFormType == NodeFormType.SheetTree || nd.HisFormType == NodeFormType.SheetAutoTree)
                return "url@../../../MyFlowTreeReadonly.htm?3=4&FK_MapData=" + nd.NodeFrmID + "&OID=" + wk.OID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + nd.NodeID + "&PK=OID&PKVal=" + wk.OID + "&IsEdit=0&IsLoadData=0&IsReadonly=1";

            if (nd.HisFormType == NodeFormType.FreeForm)
            {
                MapData md = new MapData(nd.NodeFrmID);
                if (md.HisFrmType != FrmType.FreeFrm)
                {
                    md.HisFrmType = FrmType.FreeFrm;
                    md.Update();
                }
            }
            else
            {
                nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.
                MapData md = new MapData(nd.NodeFrmID);
                if (md.HisFrmType != FrmType.FoolForm)
                {
                    md.HisFrmType = FrmType.FoolForm;
                    md.Update();
                }
            }

            //加入是累加表单的标志，目的是让附件可以看到.
            string endUrl = "";
            if (nd.HisFormType == NodeFormType.FoolTruck)
                endUrl = "&FormType=10&FromWorkOpt=" + this.GetRequestVal("FromWorkOpt");

            return "url@./CCForm/Frm.htm?FK_MapData=" + nd.NodeFrmID + "&OID=" + wk.OID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + nd.NodeID + "&PK=OID&PKVal=" + wk.OID + "&IsEdit=0&IsLoadData=0&IsReadonly=1" + endUrl;
        }
        /// <summary>
        /// 草稿
        /// </summary>
        /// <returns></returns>
        public string Draft_Init()
        {
            DataTable dt = null;
            dt = BP.WF.Dev2Interface.DB_GenerDraftDataTable();
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 删除草稿.
        /// </summary>
        /// <returns></returns>
        public string Draft_Delete()
        {
          return BP.WF.Dev2Interface.Flow_DoDeleteDraft(this.FK_Flow, this.WorkID, false);
        }
        /// <summary>
        /// 获得会签列表
        /// </summary>
        /// <returns></returns>
        public string HuiQianList_Init()
        {
            string sql = "SELECT A.WorkID, A.Title,A.FK_Flow, A.FlowName, A.Starter, A.StarterName, A.Sender, A.Sender,A.FK_Node,A.NodeName,A.SDTOfNode,A.TodoEmps";
            sql += " FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID and a.FK_Node=b.FK_Node AND B.IsPass=90 AND B.FK_Emp='"+BP.Web.WebUser.No+"'";

            DataTable dt=DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";

                dt.Columns["STARTER"].ColumnName = "Starter";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";

                dt.Columns["SENDER"].ColumnName = "Sender";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["NODENAME"].ColumnName = "NodeName";
                dt.Columns["SDTOFNODE"].ColumnName = "SDTOfNode";
                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps";
            }
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 初始化待办.
        /// </summary>
        /// <returns></returns>
        public string Todolist_Init()
        {
            WF_App_ACE en = new WF_App_ACE(context);
            return en.Todolist_Init();
        }
        /// <summary>
        /// 获得授权人的待办.
        /// </summary>
        /// <returns></returns>
        public string Todolist_Author()
        {
            DataTable dt = null;
            dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(this.No, this.FK_Node);

            //转化大写的toJson.
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string TodolistOfAuth_Init()
        {
            return "err@尚未重构完成.";
           
            //DataTable dt = null;
            //foreach (BP.WF.Port.WFEmp item in ems)
            //{
            //    if (dt == null)
            //    {
            //        dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(item.No, null);
            //    }
            //    else
            //    {
            //    }
            //}

          //    return BP.Tools.Json.DataTableToJson(dt, false);

            //string fk_emp = this.FK_Emp;
            //if (fk_emp == null)
            //{
            //    //移除不需要前台看到的数据.
            //    DataTable dt = ems.ToDataTableField();
            //    dt.Columns.Remove("SID");
            //    dt.Columns.Remove("Stas");
            //    dt.Columns.Remove("Depts");
            //    dt.Columns.Remove("Msg");
            //    return BP.Tools.Json.DataTableToJson(dt, false);
            //}

        }
         /// <summary>
        /// 获得挂起.
        /// </summary>
        /// <returns></returns>
        public string HungUpList_Init()
        {
            DataTable dt = null;
            dt = BP.WF.Dev2Interface.DB_GenerHungUpList();
            
            //转化大写的toJson.
            return BP.Tools.Json.ToJson(dt);
        }
        #endregion 获得列表.


        #region 共享任务池.
        /// <summary>
        /// 初始化共享任务
        /// </summary>
        /// <returns></returns>
        public string TaskPoolSharing_Init()
        {
           DataTable dt = BP.WF.Dev2Interface.DB_TaskPool();

           return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 申请任务.
        /// </summary>
        /// <returns></returns>
        public string TaskPoolSharing_Apply()
        {
            bool b = BP.WF.Dev2Interface.Node_TaskPoolTakebackOne(this.WorkID);
            if (b == true)
                return "申请成功.";
            else
                return "err@申请失败...";
        }
        /// <summary>
        /// 我申请下来的任务
        /// </summary>
        /// <returns></returns>
        public string TaskPoolApply_Init()
        {
            DataTable dt = BP.WF.Dev2Interface.DB_TaskPoolOfMyApply();

            return BP.Tools.Json.ToJson(dt);
        }
        public string TaskPoolApply_PutOne()
        {
            BP.WF.Dev2Interface.Node_TaskPoolPutOne(this.WorkID);
            return "放入成功,其他的同事可以看到这件工作.您可以在任务池里看到它并重新申请下来.";
        }
        #endregion

        #region 登录相关.
        /// <summary>
        /// 返回当前会话信息.
        /// </summary>
        /// <returns></returns>
        public string Login_Init()
        {
            Hashtable ht = new Hashtable();

            if (BP.Web.WebUser.NoOfRel == null)
                ht.Add("UserNo", "");
            else
                ht.Add("UserNo", BP.Web.WebUser.No);

            if (BP.Web.WebUser.IsAuthorize)
                ht.Add("Auth", BP.Web.WebUser.Auth);
            else
                ht.Add("Auth", "");
            return BP.Tools.FormatToJson.ToJson(ht);
        }
        /// <summary>
        /// 执行登录.
        /// </summary>
        /// <returns></returns>
        public string LoginSubmit()
        {
            BP.Port.Emp emp = new BP.Port.Emp();
            emp.No = this.GetValFromFrmByKey("TB_UserNo");

            if (emp.RetrieveFromDBSources() == 0)
                return "err@用户名或密码错误.";
            string pass = this.GetValFromFrmByKey("TB_Pass");
            if (emp.Pass.Equals(pass) == false)
                return "err@用户名或密码错误.";

            //让其登录.
            string sid = BP.WF.Dev2Interface.Port_Login(emp.No);
            return sid;
        }
        /// <summary>
        /// 执行授权登录
        /// </summary>
        /// <returns></returns>
        public string LoginAs()
        {
            BP.WF.Port.WFEmp wfemp = new BP.WF.Port.WFEmp(this.No);
            if (wfemp.AuthorIsOK == false)
                return "err@授权登录失败！";
            BP.Port.Emp emp1 = new BP.Port.Emp(this.No);
            BP.Web.WebUser.SignInOfGener(emp1, "CH", false, false, BP.Web.WebUser.No, BP.Web.WebUser.Name);
            return "success@授权登录成功！";
        }
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="UserNo"></param>
        /// <param name="Author"></param>
        /// <returns></returns>
        public string AuthExitAndLogin(string UserNo, string Author)
        {
            string msg = "suess@退出成功！";
            try
            {
                BP.Port.Emp emp = new BP.Port.Emp(UserNo);
                //首先退出
                BP.Web.WebUser.Exit();
                //再进行登录
                BP.Port.Emp emp1 = new BP.Port.Emp(Author);
                BP.Web.WebUser.SignInOfGener(emp1, "CH", false, false, null, null);
            }
            catch (Exception ex)
            {
                msg = "err@退出时发生错误。" + ex.Message;
            }
            return msg;
        }
        /// <summary>
        /// 获取授权人列表
        /// </summary>
        /// <returns></returns>
        public string Load_Author()
        {
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable("SELECT * FROM WF_EMP WHERE AUTHOR='" + BP.Web.WebUser.No + "'");

            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns["NO"].ColumnName = "NO";
                dt.Columns["NAME"].ColumnName = "Name";
                dt.Columns["NO"].ColumnName = "NO";


            }
            return BP.Tools.FormatToJson.ToJson(dt);
        }
        /// <summary>
        /// 当前登陆人是否有授权
        /// </summary>
        /// <returns></returns>
        public string IsHaveAuthor()
        {
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable("SELECT * FROM WF_EMP WHERE AUTHOR='" + BP.Web.WebUser.No + "'");
            WFEmp em = new WFEmp();
            em.Retrieve(WFEmpAttr.Author, BP.Web.WebUser.No);

            if (dt.Rows.Count > 0 && BP.Web.WebUser.IsAuthorize == false)
                return "suess@有授权";
            else
                return "err@没有授权";
        }
        /// <summary>
        /// 退出.
        /// </summary>
        /// <returns></returns>
        public string LoginExit()
        {
            BP.WF.Dev2Interface.Port_SigOut();
            return null;
        }
        /// <summary>
        /// 授权退出.
        /// </summary>
        /// <returns></returns>
        public string AuthExit()
        {
            return this.AuthExitAndLogin(this.No, BP.Web.WebUser.Auth);
        }
        #endregion 登录相关.

        /// <summary>
        /// 获得抄送列表
        /// </summary>
        /// <returns></returns>
        public string CC_Init()
        {
            string sta = this.GetRequestVal("Sta");
            if (sta == null || sta == "")
                sta = "-1";

            int pageSize = 6;// int.Parse(pageSizeStr);

            string pageIdxStr = this.GetRequestVal("PageIdx");
            if (pageIdxStr == null)
                pageIdxStr = "1";
            int pageIdx = int.Parse(pageIdxStr);

            //实体查询.
            //BP.WF.SMSs ss = new BP.WF.SMSs();
            //BP.En.QueryObject qo = new BP.En.QueryObject(ss);

            System.Data.DataTable dt = null;
            if (sta == "-1")
                dt = BP.WF.Dev2Interface.DB_CCList(BP.Web.WebUser.No);
            if (sta == "0")
                dt = BP.WF.Dev2Interface.DB_CCList_UnRead(BP.Web.WebUser.No);
            if (sta == "1")
                dt = BP.WF.Dev2Interface.DB_CCList_Read(BP.Web.WebUser.No);
            if (sta == "2")
                dt = BP.WF.Dev2Interface.DB_CCList_Delete(BP.Web.WebUser.No);

            //int allNum = qo.GetCount();
            //qo.DoQuery(BP.WF.SMSAttr.MyPK, pageSize, pageIdx);

            return BP.Tools.Json.ToJson(dt);
        }

        #region 处理page接口.
        /// <summary>
        /// 执行的内容
        /// </summary>
        public string DoWhat
        {
            get
            {
                return this.GetRequestVal("DoWhat"); 
            }
        }
        /// <summary>
        /// 当前的用户
        /// </summary>
        public string UserNo
        {
            get
            {
                return this.GetRequestVal("UserNo"); 
            }
        }
        /// <summary>
        /// 用户的安全校验码(请参考集成章节)
        /// </summary>
        public string SID
        {
            get
            {
                return this.GetRequestVal("SID"); 
            }
        }

        public string Port_Init()
        {
            #region 安全性校验.
            if (this.UserNo == null || this.SID == null || this.DoWhat == null)
                return "err@必要的参数没有传入，请参考接口规则。";

            if (BP.WF.Dev2Interface.Port_CheckUserLogin(this.UserNo, this.SID) == false)
                return "err@非法的访问，请与管理员联系。SID=" + this.SID;

            if (DataType.IsNullOrEmpty(WebUser.No) == true || BP.Web.WebUser.No.Equals(this.UserNo) == false)
            {
                BP.WF.Dev2Interface.Port_SigOut();
                try
                {
                    BP.WF.Dev2Interface.Port_Login(this.UserNo, this.SID);
                }
                catch (Exception ex)
                {
                    return "err@安全校验出现错误:" + ex.Message;
                }
            }
            #endregion 安全性校验.

            #region 生成参数串.
            string paras = "";
            foreach (string str in this.context.Request.QueryString)
            {
                string val = this.GetRequestVal(str);
                if (val.IndexOf('@') != -1)
                    return "err@您没有能参数: [ " + str + " ," + val + " ] 给值 ，URL 将不能被执行。";

                switch (str)
                {
                    case DoWhatList.DoNode:
                    case DoWhatList.Emps:
                    case DoWhatList.EmpWorks:
                    case DoWhatList.FlowSearch:
                    case DoWhatList.Login:
                    case DoWhatList.MyFlow:
                    case DoWhatList.MyWork:
                    case DoWhatList.Start:
                    case DoWhatList.Start5:
                    case DoWhatList.StartSimple:
                    case DoWhatList.FlowFX:
                    case DoWhatList.DealWork:
                    case "FK_Flow":
                    case "WorkID":
                    case "FK_Node":
                    case "SID":
                        break;
                    default:
                        paras += "&" + str + "=" + val;
                        break;
                }
            }
            string nodeID = int.Parse(this.FK_Flow + "01").ToString();
            #endregion 生成参数串.


            //发起流程.
            if (this.DoWhat.Equals("StartClassic") == true)
            {
                if (this.FK_Flow == null)
                    return "url@./AppClassic/Home.htm";
                else
                    return "url@./AppClassic/Home.htm?FK_Flow=" + this.FK_Flow + paras + "&FK_Node=" + nodeID;
            }

            //打开工作轨迹。
            if (this.DoWhat.Equals(DoWhatList.OneWork) == true)
            {
                if (this.FK_Flow == null || this.WorkID == null)
                    throw new Exception("@参数 FK_Flow 或者 WorkID 为 Null 。");
                return "url@WFRpt.htm?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&o2=1" + paras;
            }

            //发起页面.
            if (this.DoWhat.Equals(DoWhatList.Start) == true)
            {
                if (this.FK_Flow == null)
                    return "url@Start.htm";
                else
                    return "url@MyFlow.htm?FK_Flow=" + this.FK_Flow + paras + "&FK_Node=" + nodeID;
            }

            //处理工作.
            if (this.DoWhat.Equals(DoWhatList.DealWork) == true)
            {
                if (DataType.IsNullOrEmpty(this.FK_Flow) || this.WorkID == 0)
                    return "err@参数 FK_Flow 或者 WorkID 为Null 。";

                return "url@MyFlow.htm?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&o2=1" + paras;
            }

            //请求在途.
            if (this.DoWhat.Equals(DoWhatList.Runing) == true)
            {
                return "url@Runing.htm?FK_Flow=" + this.FK_Flow;
            }

            //请求待办。
            if (this.DoWhat.Equals(DoWhatList.EmpWorks) == true || this.DoWhat.Equals("Todolist") == true)
            {
                if (DataType.IsNullOrEmpty(this.FK_Flow))
                    return "url@Todolist.htm";
                else
                    return "url@Todolist.htm?FK_Flow=" + this.FK_Flow;
            }

            //请求流程查询。
            if (this.DoWhat.Equals(DoWhatList.FlowSearch) == true)
            {
                if (DataType.IsNullOrEmpty(this.FK_Flow))
                    return "url@./RptSearch/Default.htm";
                else
                    return "url@./RptDfine/FlowSearch.htm?2=1&FK_Flow=001&EnsName=ND" + int.Parse(this.FK_Flow) + "Rpt" + paras;
            }

            //流程查询小页面.
            if (this.DoWhat.Equals(DoWhatList.FlowSearchSmall) == true)
            {
                if (this.FK_Flow == null)
                    return "url@./RptSearch/Default.htm";
                else
                    return "url./Comm/Search.htm?EnsName=ND" + int.Parse(this.FK_Flow) + "Rpt" + paras;
            }

            //打开消息.
            if (this.DoWhat.Equals(DoWhatList.DealMsg) == true)
            {
                string guid = this.GetRequestVal("GUID");
                BP.WF.SMS sms = new SMS();
                sms.MyPK = guid;
                sms.Retrieve();

                //判断当前的登录人员.
                if (BP.Web.WebUser.No != sms.SendToEmpNo)
                    BP.WF.Dev2Interface.Port_Login(sms.SendToEmpNo);

                BP.DA.AtPara ap = new AtPara(sms.AtPara);
                switch (sms.MsgType)
                {
                    case SMSMsgType.SendSuccess: // 发送成功的提示.

                        if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(ap.GetValStrByKey("FK_Flow"),
                            ap.GetValIntByKey("FK_Node"), ap.GetValInt64ByKey("WorkID"), BP.Web.WebUser.No) == true)
                            return "url@MyFlow.htm?FK_Flow=" + ap.GetValStrByKey("FK_Flow") + "&WorkID=" + ap.GetValStrByKey("WorkID") + "&o2=1" + paras;
                        else
                            return "url@WFRpt.htm?FK_Flow=" + ap.GetValStrByKey("FK_Flow") + "&WorkID=" + ap.GetValStrByKey("WorkID") + "&o2=1" + paras;
                    default: //其他的情况都是查看工作报告.
                        return "url@WFRpt.htm?FK_Flow=" + ap.GetValStrByKey("FK_Flow") + "&WorkID=" + ap.GetValStrByKey("WorkID") + "&o2=1" + paras;
                }
            }

            return "err@没有约定的标记:DoWhat=" + this.DoWhat;
        }
        #endregion 处理page接口.

    }
}
