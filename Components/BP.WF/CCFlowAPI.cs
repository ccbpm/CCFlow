using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.En;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;
using BP.Port;
using System.Drawing.Imaging;
using System.Drawing;
using System.Configuration;
using BP.Tools;
using BP.Difference;

namespace BP.WF
{
    /// <summary>
    /// 流程的API.
    /// </summary>
    public class CCFlowAPI
    {
        /// <summary>
        /// 产生一个 WorkNode
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="Node">节点</param>
        /// <param name="workID">工作ID</param>
        /// <param name="fid">FID</param>
        /// <param name="userNo">用户编号</param>
        /// <returns>返回dataset</returns>
        public static DataSet GenerWorkNode(string fk_flow, Node nd, Int64 workID, Int64 fid, string userNo, Int64 realWorkID, string fromWorkOpt = "0",
            bool isView = false)
        {
            try
            {
                nd.WorkID = workID; //为获取表单ID提供参数.

                Work wk = nd.HisWork;
                wk.OID = workID;
                wk.RetrieveFromDBSources();

                string frmID = nd.NodeFrmID;
                //根据表单存储的数据获取获取使用表单的版本号
                int frmVer = 0;
                if (wk.EnMap.Attrs.Contains("AtPara") == true)
                {
                    frmVer = wk.FrmVer;
                    if (frmVer != 0)
                    {
                        frmID = nd.NodeFrmID + "." + frmVer;
                        if (nd.FormType != NodeFormType.FoolTruck)
                        {
                            wk = new BP.WF.GEWork(nd.NodeID, frmID);
                            wk.OID = workID;
                            wk.RetrieveFromDBSources();
                        }

                    }
                }


                MapData md = new MapData(frmID);



                //定义变量，为绑定独立表单设置单据编号.
                string billNo = null; //定义单据编号.
                string billNoField = null; //定义单据编号字段.


                // 第1.2: 调用,处理用户定义的业务逻辑.
                string sendWhen = ExecEvent.DoNode(EventListNode.FrmLoadBefore, nd,
                    wk, null);

                //获得表单模版.
                DataSet myds = BP.Sys.CCFormAPI.GenerHisDataSet(frmID, nd.Name);
                DataTable athdt = myds.Tables["Sys_FrmAttachment"];
                if (frmVer != 0 && athdt.Rows.Count != 0)
                {
                    DataTable gfdt = myds.Tables["Sys_GroupField"];
                    foreach (DataRow dr in athdt.Rows)
                    {
                        DataRow[] gfr = gfdt.Select("CtrlID='" + dr["MyPK"] + "'");
                        if (gfr.Length != 0)
                            gfr[0]["CtrlID"] = nd.NodeFrmID + "_" + dr["NoOfObj"];
                        dr["MyPK"] = nd.NodeFrmID + "_" + dr["NoOfObj"];

                    }

                }

                //更换表单的名字.
                if (DataType.IsNullOrEmpty(nd.NodeFrmID) == false
                    && (nd.HisFormType == NodeFormType.FoolForm || nd.HisFormType == NodeFormType.Develop))
                {
                    string realName = myds.Tables["Sys_MapData"].Rows[0]["Name"] as string;
                    if (DataType.IsNullOrEmpty(realName) == true)
                    {
                        myds.Tables["Sys_MapData"].Rows[0]["Name"] = nd.Name;
                    }
                }

                #region 处理表单权限控制方案: 如果是绑定单个表单的时候. 
                /*处理表单权限控制方案: 如果是绑定单个表单的时候. */

                //这两个变量在累加表单用到.
                FrmNode frmNode = new FrmNode();
                Flow flow = new Flow(fk_flow);
                myds.Tables.Add(flow.ToDataTableField("WF_Flow"));

                if (nd.HisFormType == NodeFormType.RefOneFrmTree
                    || nd.HisFormType == NodeFormType.FoolTruck
                    || flow.FlowDevModel == FlowDevModel.JiJian)
                {
                    frmNode.Retrieve(FrmNodeAttr.FK_Frm, nd.NodeFrmID, FrmNodeAttr.FK_Node, nd.NodeID);

                    if (DataType.IsNullOrEmpty(frmNode.MyPK) == false && frmNode.FrmSln != 0)
                    {
                        FrmFields fls = new FrmFields(nd.NodeFrmID, frmNode.FK_Node);
                        foreach (FrmField item in fls)
                        {
                            foreach (DataRow dr in myds.Tables["Sys_MapAttr"].Rows)
                            {
                                string keyOfEn = dr["KeyOfEn"].ToString();
                                if (keyOfEn.Equals(item.KeyOfEn) == false)
                                    continue;

                                if (item.IsSigan == true)
                                    item.setUIIsEnable(false);

                                if (item.UIIsEnable == true)
                                    dr["UIIsEnable"] = 1;
                                else
                                    dr["UIIsEnable"] = 0;

                                if (item.IsNotNull == true)
                                    dr["UIIsInput"] = 1;
                                else
                                    dr["UIIsInput"] = 0;

                                if (item.UIVisible == true)
                                    dr["UIVisible"] = 1;
                                else
                                    dr["UIVisible"] = 0;

                                if (item.IsSigan == true)
                                    dr["IsSigan"] = 1;
                                else
                                    dr["IsSigan"] = 0;

                                dr["DefVal"] = item.DefVal;
                            }
                        }

                        //从表权限的设置
                        MapDtls mapdtls = new MapDtls();
                        mapdtls.Retrieve(MapDtlAttr.FK_MapData, nd.NodeFrmID + "_" + frmNode.FK_Node);
                        foreach (DataRow dr in myds.Tables["Sys_MapDtl"].Rows)
                        {
                            foreach (MapDtl mapDtl in mapdtls)
                            {
                                string no = dr["No"].ToString() + "_" + frmNode.FK_Node;
                                if (no.Equals(mapDtl.No) == true)
                                {
                                    dr["IsView"] = mapDtl.IsView == true ? 1 : 0;
                                    break;
                                }

                            }
                        }
                    }
                }
                else
                {
                    frmNode.IsEnableLoadData = true;
                }
                #endregion 处理表单权限控制方案: 如果是绑定单个表单的时候. 

                //把流程信息表发送过去.
                GenerWorkFlow gwf = new GenerWorkFlow();
                gwf.WorkID = realWorkID;
                gwf.RetrieveFromDBSources();
                myds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));

                //加入WF_Node.
                DataTable WF_Node = nd.ToDataTableField("WF_Node");
                myds.Tables.Add(WF_Node);

                #region 加入组件的状态信息, 在解析表单的时候使用.
                FrmNodeComponent fnc = new FrmNodeComponent(nd.NodeID);
                nd.WorkID = workID; //为获取表单ID提供参数.


                bool isHaveSubFlow = false;
                #region 没有审核组件分组就增加上审核组件分组. 
                if (nd.NodeFrmID.Equals("ND" + nd.NodeID) == true || flow.FlowDevModel == FlowDevModel.JiJian ||
                    (nd.HisFormType == NodeFormType.RefOneFrmTree && DataType.IsNullOrEmpty(frmNode.MyPK) == false))
                {
                    bool isHaveFWC = false;
                    //绑定表单库中的表单
                    if ((DataType.IsNullOrEmpty(frmNode.MyPK) == false
                        && frmNode.IsEnableFWC != FrmWorkCheckSta.Disable) || ((nd.NodeFrmID == "ND" + nd.NodeID || flow.FlowDevModel == FlowDevModel.JiJian) && nd.FrmWorkCheckSta != FrmWorkCheckSta.Disable))
                        isHaveFWC = true;

                    if ((nd.FormType == NodeFormType.FoolForm
                        || frmNode.HisFrmType == FrmType.FoolForm) && isHaveFWC == true)
                    {
                        //判断是否是傻瓜表单，如果是，就要判断该傻瓜表单是否有审核组件groupfield ,没有的话就增加上.
                        DataTable gf = myds.Tables["Sys_GroupField"];
                        bool isHave = false;
                        foreach (DataRow dr in gf.Rows)
                        {
                            string cType = dr["CtrlType"] as string;
                            if (cType == null)
                                continue;
                            if (cType.Equals("FWC") == true)
                                isHave = true;
                            if (cType.Equals("SubFlow") == true)
                                isHaveSubFlow = true;
                        }

                        if (isHave == false)
                        {
                            DataRow dr = gf.NewRow();

                            nd.WorkID = workID; //为获取表单ID提供参数.
                            dr[GroupFieldAttr.OID] = 100;
                            dr[GroupFieldAttr.FrmID] = nd.NodeFrmID;
                            dr[GroupFieldAttr.CtrlType] = "FWC";
                            dr[GroupFieldAttr.CtrlID] = "FWCND" + nd.NodeID;
                            dr[GroupFieldAttr.Idx] = 100;
                            dr[GroupFieldAttr.Lab] = "审核信息";
                            gf.Rows.Add(dr);

                            myds.Tables.Remove("Sys_GroupField");
                            myds.Tables.Add(gf);

                            //更新,为了让其自动增加审核分组.
                            BP.WF.Template.FrmNodeComponent refFnc = new FrmNodeComponent(nd.NodeID);
                            refFnc.Update();
                        }
                    }
                }
                #endregion 没有审核组件分组就增加上审核组件分组.
                #region 增加父子流程组件
                if (nd.HisFormType == NodeFormType.RefOneFrmTree && DataType.IsNullOrEmpty(frmNode.MyPK) == false && frmNode.SFSta != FrmSubFlowSta.Disable)
                {
                    DataTable gf = myds.Tables["Sys_GroupField"];

                    if (isHaveSubFlow == false)
                    {
                        DataRow dr = gf.NewRow();

                        nd.WorkID = workID; //为获取表单ID提供参数.
                        dr[GroupFieldAttr.OID] = 120;
                        dr[GroupFieldAttr.FrmID] = nd.NodeFrmID;
                        dr[GroupFieldAttr.CtrlType] = "SubFlow";
                        dr[GroupFieldAttr.CtrlID] = "SubFlowND" + nd.NodeID;
                        dr[GroupFieldAttr.Idx] = 120;
                        dr[GroupFieldAttr.Lab] = "父子流程";
                        gf.Rows.Add(dr);

                        myds.Tables.Remove("Sys_GroupField");
                        myds.Tables.Add(gf);

                        //更新,为了让其自动增加审核分组.
                        BP.WF.Template.FrmNodeComponent refFnc = new FrmNodeComponent(nd.NodeID);
                        refFnc.Update();
                    }
                }


                #endregion 增加父子流程组件

                //把审核组件信息，放入ds.
                myds.Tables.Add(fnc.ToDataTableField("WF_FrmNodeComponent"));

                #endregion 加入组件的状态信息, 在解析表单的时候使用.

                #region 处理累加表单增加 groupfields
                if (nd.FormType == NodeFormType.FoolTruck && nd.IsStartNode == false
                    && DataType.IsNullOrEmpty(wk.HisPassedFrmIDs) == false)
                {

                    #region 处理字段分组排序. groupfields
                    //查询所有的分组, 如果是查看表单的方式，就不应该把当前的表单显示出来.
                    string myFrmIDs = "";
                    if (fromWorkOpt.Equals("1") == true)
                    {
                        if (gwf.WFState == WFState.Complete)
                            myFrmIDs = wk.HisPassedFrmIDs + ",'ND" + nd.NodeID + "'";
                        else
                            myFrmIDs = wk.HisPassedFrmIDs; //流程未完成并且是查看表单的情况.
                    }
                    else
                    {
                        myFrmIDs = wk.HisPassedFrmIDs + ",'ND" + nd.NodeID + "'";
                    }

                    GroupFields gfs = new GroupFields();
                    gfs.RetrieveIn(GroupFieldAttr.FrmID, "(" + myFrmIDs + ")");

                    //按照时间的顺序查找出来 ids .
                    string sqlOrder = "SELECT OID FROM  Sys_GroupField WHERE   FrmID IN (" + myFrmIDs + ")";
                    string orderMyFrmIDs = myFrmIDs.Replace("'", "");
                    if (SystemConfig.AppCenterDBType == DBType.Oracle)
                    {
                        sqlOrder += " ORDER BY INSTR('" + orderMyFrmIDs + "',FrmID) , Idx";
                    }

                    if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                    {
                        sqlOrder += " ORDER BY CHARINDEX(FrmID, '" + orderMyFrmIDs + "'), Idx";
                    }

                    if (SystemConfig.AppCenterDBType == DBType.MySQL)
                    {
                        sqlOrder += " ORDER BY INSTR('" + orderMyFrmIDs + "', FrmID ), Idx";
                    }
                    if (SystemConfig.AppCenterDBType == DBType.PostgreSQL || SystemConfig.AppCenterDBType == DBType.UX)
                    {
                        sqlOrder += " ORDER BY POSITION(FrmID  IN '" + orderMyFrmIDs + "'), Idx";
                    }

                    if (SystemConfig.AppCenterDBType == DBType.DM)
                    {
                        sqlOrder += " ORDER BY POSITION(FrmID  IN '" + orderMyFrmIDs + "'), Idx";
                    }

                    DataTable dtOrder = DBAccess.RunSQLReturnTable(sqlOrder);

                    //创建容器,把排序的分组放入这个容器.
                    GroupFields gfsNew = new GroupFields();

                    //遍历查询出来的分组.
                    //只能增加一个审核分组
                    GroupField FWCG = null;
                    foreach (DataRow dr in dtOrder.Rows)
                    {
                        string pkOID = dr[0].ToString();
                        GroupField mygf = gfs.GetEntityByKey(pkOID) as GroupField;
                        if (mygf.CtrlType.Equals("FWC"))
                        {
                            FWCG = mygf;
                            continue;
                        }

                        gfsNew.AddEntity(mygf); //把分组字段加入里面去.
                    }
                    if (FWCG != null)
                        gfsNew.AddEntity(FWCG);

                    DataTable dtGF = gfsNew.ToDataTableField("Sys_GroupField");
                    myds.Tables.Remove("Sys_GroupField");
                    myds.Tables.Add(dtGF);
                    #endregion 处理字段分组排序. groupfields

                    #region 处理 mapattrs
                    //求当前表单的字段集合.
                    MapAttrs mattrs = new MapAttrs();
                    QueryObject qo = new QueryObject(mattrs);
                    qo.AddWhere(MapAttrAttr.FK_MapData, "ND" + nd.NodeID);
                    qo.addOrderBy(MapAttrAttr.Idx);
                    qo.DoQuery();

                    //获取走过节点的表单方案
                    MapAttrs attrsLeiJia = new MapAttrs();

                    //存在表单方案只读
                    string sql1 = "Select FK_Frm From WF_FrmNode Where FK_Frm In(" + wk.HisPassedFrmIDs + ") And FrmSln=" + (int)FrmSln.Readonly + " And FK_Node=" + nd.NodeID;
                    DataTable dt1 = DBAccess.RunSQLReturnTable(sql1);
                    if (dt1.Rows.Count > 0)
                    {
                        //获取节点
                        string nodes = "";
                        foreach (DataRow dr in dt1.Rows)
                            nodes += "'" + dr[0].ToString() + "',";

                        nodes = nodes.Substring(0, nodes.Length - 1);
                        qo = new QueryObject(attrsLeiJia);
                        qo.AddWhere(MapAttrAttr.FK_MapData, " IN ", "(" + nodes + ")");
                        qo.addOrderBy(MapAttrAttr.Idx);
                        qo.DoQuery();

                        foreach (MapAttr item in attrsLeiJia)
                        {
                            if (item.KeyOfEn.Equals("RDT") || item.KeyOfEn.Equals("Rec"))
                                continue;
                            item.setUIIsEnable(false); //设置为只读的.
                            mattrs.AddEntity(item);
                        }

                    }

                    //存在表单方案默认
                    sql1 = "Select FK_Frm From WF_FrmNode Where FK_Frm In(" + wk.HisPassedFrmIDs + ") And FrmSln=" + (int)FrmSln.Default + " And FK_Node=" + nd.NodeID;
                    dt1 = DBAccess.RunSQLReturnTable(sql1);
                    if (dt1.Rows.Count > 0)
                    {
                        //获取节点
                        string nodes = "";
                        foreach (DataRow dr in dt1.Rows)
                            nodes += "'" + dr[0].ToString() + "',";

                        nodes = nodes.Substring(0, nodes.Length - 1);
                        qo = new QueryObject(attrsLeiJia);
                        qo.AddWhere(MapAttrAttr.FK_MapData, " IN ", "(" + nodes + ")");
                        qo.addOrderBy(MapAttrAttr.Idx);
                        qo.DoQuery();

                        foreach (MapAttr item in attrsLeiJia)
                        {
                            if (item.KeyOfEn.Equals("RDT") || item.KeyOfEn.Equals("Rec"))
                                continue;
                            mattrs.AddEntity(item);
                        }

                    }

                    //存在表单方案自定义
                    sql1 = "Select FK_Frm From WF_FrmNode Where FK_Frm In(" + wk.HisPassedFrmIDs + ") And FrmSln=" + (int)FrmSln.Self + " And FK_Node=" + nd.NodeID;
                    dt1 = DBAccess.RunSQLReturnTable(sql1);

                    if (dt1.Rows.Count > 0)
                    {
                        //获取节点
                        string nodes = "";
                        foreach (DataRow dr in dt1.Rows)
                            nodes += "'" + dr[0].ToString() + "',";

                        nodes = nodes.Substring(0, nodes.Length - 1);
                        qo = new QueryObject(attrsLeiJia);
                        qo.AddWhere(MapAttrAttr.FK_MapData, " IN ", "(" + nodes + ")");
                        qo.addOrderBy(MapAttrAttr.Idx);
                        qo.DoQuery();

                        //获取累加表单的权限
                        FrmFields fls = new FrmFields();
                        qo = new QueryObject(fls);
                        qo.AddWhere(FrmFieldAttr.FK_MapData, " IN ", "(" + nodes + ")");
                        qo.addAnd();
                        qo.AddWhere(FrmFieldAttr.EleType, FrmEleType.Field);
                        qo.addAnd();
                        qo.AddWhere(FrmFieldAttr.FK_Node, nd.NodeID);
                        qo.DoQuery();

                        foreach (MapAttr attr in attrsLeiJia)
                        {
                            if (attr.KeyOfEn.Equals("RDT") || attr.KeyOfEn.Equals("Rec"))
                                continue;

                            FrmField frmField = null;
                            foreach (FrmField item in fls)
                            {
                                if (attr.KeyOfEn == item.KeyOfEn)
                                {
                                    frmField = item;
                                    break;
                                }
                            }
                            if (frmField != null)
                            {
                                if (frmField.IsSigan)
                                    attr.setUIIsEnable(false);

                                attr.setUIIsEnable(frmField.UIIsEnable);
                                attr.setUIVisible(frmField.UIVisible);
                                attr.IsSigan = frmField.IsSigan;
                                attr.setDefValReal(frmField.DefVal);
                            }
                            mattrs.AddEntity(attr);
                        }

                    }

                    //替换掉现有的.
                    myds.Tables.Remove("Sys_MapAttr"); //移除.
                    myds.Tables.Add(mattrs.ToDataTableField("Sys_MapAttr")); //增加.
                    #endregion 处理mapattrs

                    #region 把枚举放入里面去.
                    myds.Tables.Remove("Sys_Enum");

                    myFrmIDs = wk.HisPassedFrmIDs + ",'ND" + nd.NodeID + "'";
                    SysEnums enums = new SysEnums();
                    enums.RetrieveInSQL(SysEnumAttr.EnumKey,
                            "SELECT UIBindKey FROM Sys_MapAttr WHERE FK_MapData in(" + myFrmIDs + ")", SysEnumAttr.IntKey);

                    // 加入最新的枚举.
                    myds.Tables.Add(enums.ToDataTableField("Sys_Enum"));
                    #endregion 把枚举放入里面去.

                    #region  MapExt .
                    myds.Tables.Remove("Sys_MapExt");

                    // 把扩展放入里面去.
                    myFrmIDs = wk.HisPassedFrmIDs + ",'ND" + nd.NodeID + "'";
                    MapExts exts = new MapExts();
                    qo = new QueryObject(exts);
                    qo.AddWhere(MapExtAttr.FK_MapData, " IN ", "(" + myFrmIDs + ")");
                    qo.DoQuery();

                    // 加入最新的MapExt.
                    myds.Tables.Add(exts.ToDataTableField("Sys_MapExt"));
                    #endregion  MapExt .

                    #region  MapDtl .
                    myds.Tables.Remove("Sys_MapDtl");

                    //把从表放里面
                    myFrmIDs = wk.HisPassedFrmIDs + ",'ND" + nd.NodeID + "'";
                    MapDtls dtls = new MapDtls();
                    qo = new QueryObject(dtls);
                    qo.AddWhere(MapDtlAttr.FK_MapData, " IN ", "(" + myFrmIDs + ")");
                    qo.addAnd();
                    qo.AddWhere(MapDtlAttr.FK_Node, 0);

                    qo.DoQuery();

                    // 加入最新的MapDtl.
                    myds.Tables.Add(dtls.ToDataTableField("Sys_MapDtl"));
                    #endregion  MapDtl .

                    #region  FrmAttachment .
                    myds.Tables.Remove("Sys_FrmAttachment");

                    //把附件放里面
                    myFrmIDs = wk.HisPassedFrmIDs + ",'ND" + nd.NodeID + "'";
                    FrmAttachments frmAtchs = new FrmAttachments();
                    qo = new QueryObject(frmAtchs);
                    qo.AddWhere(FrmAttachmentAttr.FK_MapData, " IN ", "(" + myFrmIDs + ")");
                    qo.addAnd();
                    qo.AddWhere(FrmAttachmentAttr.FK_Node, 0);
                    qo.DoQuery();

                    // 加入最新的Sys_FrmAttachment.
                    myds.Tables.Add(frmAtchs.ToDataTableField("Sys_FrmAttachment"));
                    #endregion  FrmAttachment .

                }
                #endregion 增加 groupfields

                #region 流程设置信息.
                if (isView == false)
                {
                    BP.WF.Dev2Interface.Node_SetWorkRead(nd.NodeID, realWorkID);
                    if (nd.IsStartNode == false)
                    {
                        if (gwf.TodoEmps.Contains(BP.Web.WebUser.No + ",") == false)
                        {
                            gwf.TodoEmps += BP.Web.WebUser.No + "," + BP.Web.WebUser.Name + ";";
                            gwf.Update();
                        }
                    }
                }
                #endregion 流程设置信息.

                #region 把主从表数据放入里面.
                //.工作数据放里面去, 放进去前执行一次装载前填充事件.

                //重设默认值.
                if (isView == false && (DataType.IsNullOrEmpty(frmNode.MyPK) == true || (DataType.IsNullOrEmpty(frmNode.MyPK) == false && frmNode.FrmSln != FrmSln.Readonly)))
                    wk.ResetDefaultVal(nd.NodeFrmID, fk_flow, nd.NodeID);

                //URL参数替换
                if (SystemConfig.IsBSsystem == true && isView == false)
                {
                    // 处理传递过来的参数。
                    foreach (string k in HttpContextHelper.RequestQueryStringKeys)
                    {
                        if (DataType.IsNullOrEmpty(k) == true)
                            continue;

                        wk.SetValByKey(k, HttpContextHelper.RequestParams(k));
                    }

                    foreach (string k in HttpContextHelper.RequestParamKeys)
                    {
                        if (DataType.IsNullOrEmpty(k) == true)
                            continue;

                        wk.SetValByKey(k, HttpContextHelper.RequestParams(k));
                    }

                    //更新到数据库里.
                    wk.DirectUpdate();
                }

                //执行表单事件
                MapExts mes = md.MapExts;
                MapExt me = null;
                string msg = null;
                if (isView == false)
                {
                    msg = ExecEvent.DoFrm(md, EventListFrm.FrmLoadBefore, wk);
                    if (DataType.IsNullOrEmpty(msg) == false)
                        throw new Exception("err@错误:" + msg);

                    //执行装载填充.
                    string mypk = MapExtXmlList.PageLoadFull + "_" + md.No;
                    if (frmNode.IsEnableLoadData == true && md.IsPageLoadFull == true)
                    {
                        me = mes.GetEntityByKey("MyPK", mypk) as MapExt;
                        //执行通用的装载方法.
                        MapAttrs attrs = md.MapAttrs;
                        MapDtls dtls = md.MapDtls;
                        wk = BP.WF.Glo.DealPageLoadFull(wk, me, attrs, dtls) as Work;
                    }
                }

                //如果是累加表单，就把整个rpt数据都放入里面去.
                if (nd.FormType == NodeFormType.FoolTruck && nd.IsStartNode == false
                  && DataType.IsNullOrEmpty(wk.HisPassedFrmIDs) == false)
                {
                    GERpt rpt = new GERpt("ND" + int.Parse(nd.FK_Flow) + "Rpt", workID);
                    rpt.Copy(wk);

                    DataTable rptdt = rpt.ToDataTableField("MainTable");
                    myds.Tables.Add(rptdt);
                }
                else
                {
                    DataTable mainTable = wk.ToDataTableField("MainTable");
                    myds.Tables.Add(mainTable);
                }
                string sql = "";
                DataTable dt = null;
                #endregion

                #region 把外键表加入 DataSet
                DataTable dtMapAttr = myds.Tables["Sys_MapAttr"];

                DataTable ddlTable = new DataTable();
                ddlTable.Columns.Add("No");

                foreach (DataRow dr in dtMapAttr.Rows)
                {
                    string lgType = dr["LGType"].ToString();
                    string uiBindKey = dr["UIBindKey"].ToString();

                    if (DataType.IsNullOrEmpty(uiBindKey) == true)
                        continue; //为空就continue.

                    if (lgType.Equals("1") == true)
                        continue; //枚举值就continue;
                    //@liuqiang, 
                    string uiIsEnable = dr["UIIsEnable"].ToString();
                    if (uiIsEnable.Equals("0") == true && lgType.Equals("2") == true)
                        continue; //如果是外键，并且是不可以编辑的状态.

                    if (uiIsEnable.Equals("0") == true && lgType.Equals("0") == true)
                        continue; //如果是外部数据源，并且是不可以编辑的状态.


                    // 检查是否有下拉框自动填充。
                    string keyOfEn = dr["KeyOfEn"].ToString();
                    string fk_mapData = dr["FK_MapData"].ToString();

                    #region 处理下拉框数据范围. for 小杨.
                    me = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.AutoFullDLL, MapExtAttr.AttrOfOper, keyOfEn) as MapExt;
                    if (me != null && myds.Tables.Contains(keyOfEn) == false)
                    {
                        string fullSQL = me.Doc.Clone() as string;
                        if (fullSQL == null)
                            throw new Exception("err@字段[" + keyOfEn + "]下拉框AutoFullDLL，没有配置SQL");

                        fullSQL = fullSQL.Replace("~", "'");
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

                    DataTable mydt = BP.Pub.PubClass.GetDataTableByUIBineKey(uiBindKey);
                    if (mydt == null)
                    {
                        DataRow ddldr = ddlTable.NewRow();
                        ddldr["No"] = uiBindKey;
                        ddlTable.Rows.Add(ddldr);
                    }
                    else
                    {
                        myds.Tables.Add(mydt);
                    }
                }
                ddlTable.TableName = "UIBindKey";
                myds.Tables.Add(ddlTable);
                #endregion End把外键表加入DataSet

                #region 处理流程-消息提示.
                if (isView == true || fromWorkOpt.Equals("1"))
                {
                    //如果是查看模式，或者是从WorkOpt打开模式,就不让其显示消息.
                }
                else
                {
                    DataTable dtAlert = new DataTable();
                    dtAlert.TableName = "AlertMsg";
                    dtAlert.Columns.Add("Title", typeof(string));
                    dtAlert.Columns.Add("Msg", typeof(string));
                    dtAlert.Columns.Add("URL", typeof(string));
                    DataRow drMsg = null;
                    int sta = gwf.GetParaInt("HungupSta");
                    switch (gwf.WFState)
                    {
                        case WFState.Runing:
                            drMsg = dtAlert.NewRow();
                            drMsg["Title"] = "挂起信息";
                            if (sta == 2 && gwf.FK_Node == gwf.GetParaInt("HungupNodeID"))
                            {
                                drMsg["Msg"] = "您的工单在挂起被拒绝，拒绝原因:" + gwf.GetParaString("HungupCheckMsg");
                                dtAlert.Rows.Add(drMsg);
                            }
                            break;
                        case WFState.Hungup:
                            if (sta == -1)
                                break;
                            drMsg = dtAlert.NewRow();
                            drMsg["Title"] = "挂起信息";
                            if (sta == 0)
                                drMsg["Msg"] = "您的工单在挂起状态，等待审批，挂起原因：" + gwf.GetParaString("HungupNote");

                            if (sta == 1)
                                drMsg["Msg"] = "您的工单在挂起获得同意.";

                            if (sta == 2)
                                drMsg["Msg"] = "您的工单在挂起被拒绝，拒绝原因:" + gwf.GetParaString("HungupCheckMsg");

                            dtAlert.Rows.Add(drMsg);
                            break;
                        case WFState.AskForReplay: // 返回加签的信息.
                            string mysql = "SELECT Msg,EmpFrom,EmpFromT,RDT FROM ND" + int.Parse(fk_flow) + "Track WHERE WorkID=" + realWorkID + " AND " + TrackAttr.ActionType + "=" + (int)ActionType.ForwardAskfor;
                            DataTable mydt = DBAccess.RunSQLReturnTable(mysql);

                            foreach (DataRow dr in mydt.Rows)
                            {
                                string msgAskFor = dr[0].ToString();
                                string worker = dr[1].ToString();
                                string workerName = dr[2].ToString();
                                string rdt = dr[3].ToString();

                                drMsg = dtAlert.NewRow();
                                drMsg["Title"] = worker + "," + workerName + "回复信息:";
                                drMsg["Msg"] = DataType.ParseText2Html(msgAskFor) + "<br>" + rdt;
                                dtAlert.Rows.Add(drMsg);
                            }
                            break;
                        case WFState.Askfor: //加签.

                            sql = "SELECT * FROM ND" + int.Parse(fk_flow) + "Track WHERE WorkID=" + realWorkID + " AND " + TrackAttr.ActionType + "=" + (int)ActionType.AskforHelp;
                            dt = DBAccess.RunSQLReturnTable(sql);
                            foreach (DataRow dr in dt.Rows)
                            {
                                string msgAskFor = dr[TrackAttr.Msg].ToString();
                                string worker = dr[TrackAttr.EmpFrom].ToString();
                                string workerName = dr[TrackAttr.EmpFromT].ToString();
                                string rdt = dr[TrackAttr.RDT].ToString();

                                drMsg = dtAlert.NewRow();
                                drMsg["Title"] = worker + "," + workerName + "请求加签:";
                                drMsg["Msg"] = DataType.ParseText2Html(msgAskFor) + "<br>" + rdt + "<a href='./WorkOpt/AskForRe.htm?FK_Flow=" + fk_flow + "&FK_Node=" + nd.NodeID + "&WorkID=" + realWorkID + "&FID=" + fid + "' >回复加签意见</a> --";
                                dtAlert.Rows.Add(drMsg);

                            }
                            // isAskFor = true;
                            break;
                        case WFState.ReturnSta:
                            /* 如果工作节点退回了*/
                            sql = "SELECT WorkID FROM WF_GenerWorkFlow WHERE FID=" + realWorkID;
                            DataTable subFlowDt = DBAccess.RunSQLReturnTable(sql);

                            sql = "SELECT  ReturnToEmp,ReturnNodeName,ReturnerName,RDT,BeiZhu FROM WF_ReturnWork where ReturnToNode=" + nd.NodeID + " AND (WorkID=" + realWorkID;
                            foreach (DataRow dRow in subFlowDt.Rows)
                            {
                                sql += " OR WorkID=" + int.Parse(dRow[0].ToString());
                            }
                            sql += ") ORDER BY RDT";

                            DataTable rwsDt = DBAccess.RunSQLReturnTable(sql);

                            if (rwsDt.Rows.Count != 0)
                            {
                                string msgInfo = "";
                                foreach (DataRow rw in rwsDt.Rows)
                                {
                                    if (WebUser.No == rw[0].ToString())
                                    {
                                        msgInfo += "来自节点：" + rw[1].ToString() + "@退回人：" + rw[2].ToString() + "@退回日期：" + rw[3].ToString();
                                        msgInfo += "@退回原因：" + rw[4].ToString();
                                        msgInfo += "<hr/>";
                                    }
                                }

                                msgInfo = msgInfo.Replace("@", "<br>");
                                if (!string.IsNullOrEmpty(msgInfo))
                                {
                                    string str = nd.ReturnAlert;
                                    if (str != "")
                                    {
                                        str = str.Replace("~", "'");
                                        str = str.Replace("@PWorkID", realWorkID.ToString());
                                        str = str.Replace("@PNodeID", nd.NodeID.ToString());
                                        str = str.Replace("@FK_Node", nd.NodeID.ToString());

                                        str = str.Replace("@PFlowNo", fk_flow);
                                        str = str.Replace("@FK_Flow", fk_flow);
                                        str = str.Replace("@PWorkID", workID.ToString());

                                        str = str.Replace("@WorkID", workID.ToString());
                                        str = str.Replace("@OID", workID.ToString());

                                        drMsg = dtAlert.NewRow();
                                        drMsg["Title"] = "退回信息";
                                        drMsg["Msg"] = msgInfo + "\t\n" + str;
                                        dtAlert.Rows.Add(drMsg);
                                    }
                                    else
                                    {
                                        drMsg = dtAlert.NewRow();
                                        drMsg["Title"] = "退回信息";
                                        drMsg["Msg"] = msgInfo + "\t\n" + str;
                                        dtAlert.Rows.Add(drMsg);
                                    }
                                }
                            }
                            break;
                        case WFState.Shift:
                            /* 判断移交过来的。 */
                            string sqlshift = "SELECT * FROM ND" + int.Parse(fk_flow) + "Track WHERE ACTIONTYPE=3 AND WorkID=" + realWorkID + " AND NDFrom='" + gwf.FK_Node + "' ORDER BY RDT DESC ";
                            DataTable dtshift = DBAccess.RunSQLReturnTable(sqlshift);

                            if (dtshift.Rows.Count >= 1)
                            {
                                msg = "";
                                drMsg = dtAlert.NewRow();
                                drMsg["Title"] = "移交信息";
                                //  msg = "<h3>移交信息 </h3><hr/>";
                                foreach (DataRow dr in dtshift.Rows)
                                {
                                    if (WebUser.No == dr[TrackAttr.EmpTo].ToString())
                                    {
                                        string empFromT = dr[TrackAttr.EmpFromT].ToString();
                                        string empToT = dr[TrackAttr.EmpToT].ToString();
                                        string msgShift = dr[TrackAttr.Msg].ToString();
                                        string rdt = dr[TrackAttr.RDT].ToString();
                                        if (msgShift == "undefined")
                                            msgShift = "无";

                                        msg += "移交人：" + empFromT + "@接受人：" + empToT + "@移交日期：" + rdt;
                                        msg += "@移交原因：" + msgShift;
                                        msg += "<hr/>";
                                    }
                                }

                                msg = msg.Replace("@", "<br>");

                                drMsg["Msg"] = msg;
                                if (!string.IsNullOrEmpty(msg))
                                {
                                    dtAlert.Rows.Add(drMsg);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    PushMsgs pms = new PushMsgs();
                    if (pms.Retrieve(PushMsgAttr.FK_Node, gwf.FK_Node,
                        PushMsgAttr.FK_Event, EventListNode.PressAfter) > 0)
                    {
                        string sqlPress = "select mobileinfo from sys_sms where msgtype='DoPress' and workid=" + gwf.WorkID + "  and isalert=0";
                        DataTable dtPress = DBAccess.RunSQLReturnTable(sqlPress);
                        foreach (DataRow item in dtPress.Rows)
                        {
                            drMsg = dtAlert.NewRow();
                            drMsg["Title"] = "催办信息";
                            drMsg["Msg"] = item["mobileinfo"].ToString();

                            dtAlert.Rows.Add(drMsg);
                        }

                        DBAccess.RunSQL("update sys_sms set isalert=1  where msgtype='DoPress' and workid=" + gwf.WorkID + "  and isalert=0");
                    }
                    //拒绝挂起
                    string sqlRejectHungup = "select mobileinfo from sys_sms where msgtype='RejectHungup' and workid=" + gwf.WorkID + "  and isalert=0";
                    DataTable dtRejectHungup = DBAccess.RunSQLReturnTable(sqlRejectHungup);
                    foreach (DataRow item in dtRejectHungup.Rows)
                    {
                        drMsg = dtAlert.NewRow();
                        drMsg["Title"] = "拒绝挂起信息";
                        drMsg["Msg"] = item["mobileinfo"].ToString();

                        dtAlert.Rows.Add(drMsg);
                    }

                    DBAccess.RunSQL("update sys_sms set isalert=1  where msgtype='RejectHungup' and workid=" + gwf.WorkID + "  and isalert=0");

                    myds.Tables.Add(dtAlert);
                }
                #endregion

                #region 增加流程节点表单绑定信息.
                myds.Tables.Add(frmNode.ToDataTableField("WF_FrmNode"));
                if (nd.HisFormType == NodeFormType.RefOneFrmTree)
                {
                    /* 独立流程节点表单. */
                    nd.WorkID = workID; //为获取表单ID ( NodeFrmID )提供参数.

                    //设置单据编号,对于绑定的表单.
                    if (nd.IsStartNode == true && DataType.IsNullOrEmpty(frmNode.BillNoField) == false)
                    {
                        DataTable dtMain = myds.Tables["MainTable"];
                        if (dtMain.Columns.Contains(frmNode.BillNoField) == true)
                        {
                            dtMain.Rows[0][frmNode.BillNoField] = wk.GetValStringByKey("BillNo");
                        }
                    }

                    if (fnc.FWCSta != FrmWorkCheckSta.Disable)
                    {

                    }

                }
                #endregion 增加流程节点表单绑定信息.

                // myds.WriteXml("c:/11.xml");

                return myds;
            }
            catch (Exception ex)
            {
                BP.DA.Log.DebugWriteError(ex.StackTrace);
                throw new Exception("generoWorkNode@" + ex.Message);
            }
        }
    }
}
