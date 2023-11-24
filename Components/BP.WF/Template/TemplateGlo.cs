using System;
using System.Collections.Generic;
using System.IO;
using BP.DA;
using BP.Sys;
using BP.Web;
using System.Data;
using BP.En;
using BP.Difference;
using BP.WF.Template.SFlow;
using BP.WF.Template.Frm;
using System.Collections.Concurrent;
using System.Threading;
using System.Web;

namespace BP.WF.Template
{
    /// <summary>
    /// 流程模版的操作
    /// </summary>
    public class TemplateGlo
    {
        /// <summary>
        /// 装载流程模板
        /// </summary>
        /// <param name="fk_flowSort">流程类别</param>
        /// <param name="path">流程名称</param>
        /// <returns></returns>
        public static Flow LoadFlowTemplate(string fk_flowSort, string path, ImpFlowTempleteModel model, string SpecialFlowNo = "", string flowName = null)
        {
            FileInfo info = new FileInfo(path);
            DataSet ds = new DataSet();
            try
            {
                ds.ReadXml(path);
            }
            catch (Exception ex)
            {
                throw new Exception("@导入流程路径:" + path + "出错：" + ex.Message);
            }


            if (ds.Tables.Contains("WF_Flow") == false)
                throw new Exception("err@导入错误，非流程模版文件" + path + "。");

            DataTable dtFlow = ds.Tables["WF_Flow"];
            Flow fl = new Flow();
            string oldFlowNo = dtFlow.Rows[0]["No"].ToString();
            string oldFlowName = dtFlow.Rows[0]["Name"].ToString();

            int oldFlowID = int.Parse(oldFlowNo);
            int iOldFlowLength = oldFlowID.ToString().Length;
            string timeKey = DateTime.Now.ToString("yyMMddhhmmss");

            #region 根据不同的流程模式，设置生成不同的流程编号.
            switch (model)
            {
                case ImpFlowTempleteModel.AsNewFlow: /*做为一个新流程. */
                    fl.No = fl.GenerNewNo;
                    fl.DoDelData();
                    fl.DoDelete(); /*删除可能存在的垃圾.*/
                    fl.Insert();
                    break;
                case ImpFlowTempleteModel.AsTempleteFlowNo: /*用流程模版中的编号*/
                    fl.No = oldFlowNo;
                    if (fl.IsExits)
                        throw new Exception("导入错误:流程模版(" + oldFlowName + ")中的编号(" + oldFlowNo + ")在系统中已经存在,流程名称为:" + dtFlow.Rows[0]["name"].ToString());
                    else
                    {
                        fl.No = oldFlowNo;
                        fl.DoDelData();
                        fl.DoDelete(); /*删除可能存在的垃圾.*/
                        fl.Insert();

                    }
                    break;
                case ImpFlowTempleteModel.OvrewaiteCurrFlowNo: /*覆盖当前的流程.*/
                    fl.No = oldFlowNo;
                    fl.DoDelData();
                    fl.DoDelete(); /*删除可能存在的垃圾.*/
                    fl.Insert();
                    break;
                case ImpFlowTempleteModel.AsSpecFlowNo:
                    if (SpecialFlowNo.Length <= 0)
                        throw new Exception("@您是按照指定的流程编号导入的，但是您没有传入正确的流程编号。");

                    fl.No = SpecialFlowNo.ToString();
                    fl.DoDelData();
                    fl.DoDelete(); /*删除可能存在的垃圾.*/
                    fl.Insert();
                    break;
                default:
                    throw new Exception("@没有判断");
            }
            #endregion 根据不同的流程模式，设置生成不同的流程编号.

            // string timeKey = fl.No;
            int idx = 0;
            string infoErr = "";
            string infoTable = "";
            int flowID = int.Parse(fl.No);

            #region 处理流程表数据
            foreach (DataColumn dc in dtFlow.Columns)
            {
                string val = dtFlow.Rows[0][dc.ColumnName] as string;
                switch (dc.ColumnName.ToLower())
                {
                    case "no":
                    case "fk_flowsort":
                        continue;
                    case "name":
                        // val = "复制:" + val + "_" + DateTime.Now.ToString("MM月dd日HH时mm分");
                        break;
                    default:
                        break;
                }
                fl.SetValByKey(dc.ColumnName, val);
            }
            fl.FlowSortNo = fk_flowSort;
            if (DBAccess.IsExitsObject(fl.PTable) == true)
            {
                fl.PTable = null;
            }
            //修改成当前登陆人所在的组织
            fl.OrgNo = WebUser.OrgNo;
            fl.SetValByKey("Creater", WebUser.No);
            if (DataType.IsNullOrEmpty(flowName) == false)
                fl.Name = flowName;
            if (fl.FlowDevModel == FlowDevModel.JiJian)
            {
                //修改FrmUrl的值
                fl.FrmUrl="ND" + Int32.Parse(fl.No) + "01";
            }
            fl.Update();
            //判断该流程是否是公文流程，存在BuessFields、FlowBuessType、FK_DocType=01
            Attrs attrs = fl.EnMap.Attrs;
            if (attrs.Contains("FlowBuessType") == true)
            {
                DBAccess.RunSQL("UPDATE WF_Flow Set BuessFields='" + fl.GetParaString("BuessFields") + "', FlowBuessType=" + fl.GetParaInt("FlowBuessType") + " ,FK_DocType='" + fl.GetParaString("FK_DocType") + "'");
            }

            #endregion 处理流程表数据

            #region 处理OID 插入重复的问题 Sys_GroupField, Sys_MapAttr.
            DataTable mydtGF = ds.Tables["Sys_GroupField"];
            DataTable myDTAttr = ds.Tables["Sys_MapAttr"];
            DataTable myDTAth = ds.Tables["Sys_FrmAttachment"];
            DataTable myDTDtl = ds.Tables["Sys_MapDtl"];
            DataTable myDFrm = ds.Tables["Sys_MapFrame"];

            foreach (DataRow dr in mydtGF.Rows)
            {
                Sys.GroupField gf = new Sys.GroupField();
                foreach (DataColumn dc in mydtGF.Columns)
                {
                    string val = dr[dc.ColumnName] as string;
                    if (val == null)
                        continue;
                    switch (dc.ColumnName.ToLower())
                    {
                        case "enname":
                        case "keyofen":
                        case "ctrlid": //升级傻瓜表单的时候,新增加的字段 add by zhoupeng 2016.11.21
                        case "frmid": //升级傻瓜表单的时候,新增加的字段 add by zhoupeng 2016.11.21
                            val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                            break;
                        default:
                            break;
                    }
                    gf.SetValByKey(dc.ColumnName, val);
                }
                int oldID = gf.OID;
                gf.OID = DBAccess.GenerOID();
                gf.DirectInsert();
                dr["OID"] = gf.OID; //给他一个新的OID.

                // 属性。
                if (myDTAttr != null && myDTAttr.Columns.Contains("GroupID"))
                {
                    foreach (DataRow dr1 in myDTAttr.Rows)
                    {
                        if (dr1["GroupID"] == null)
                            dr1["GroupID"] = 0;

                        if (dr1["GroupID"].ToString().Equals(oldID.ToString()))
                            dr1["GroupID"] = gf.OID;
                    }
                }

                // 附件。
                if (myDTAth != null && myDTAth.Columns.Contains("GroupID"))
                {
                    foreach (DataRow dr1 in myDTAth.Rows)
                    {
                        if (dr1["GroupID"] == null)
                            dr1["GroupID"] = 0;

                        if (dr1["GroupID"].ToString().Equals(oldID.ToString()))
                            dr1["GroupID"] = gf.OID;
                    }
                }

                if (myDTDtl != null && myDTDtl.Columns.Contains("GroupID"))
                {
                    // 从表。
                    foreach (DataRow dr1 in myDTDtl.Rows)
                    {
                        if (dr1["GroupID"] == null)
                            dr1["GroupID"] = 0;

                        if (dr1["GroupID"].ToString().Equals(oldID.ToString()))
                            dr1["GroupID"] = gf.OID;
                    }
                }

                // frm.
                if (myDFrm != null && myDFrm.Columns.Contains("GroupID"))
                {
                    foreach (DataRow dr1 in myDFrm.Rows)
                    {
                        if (dr1["GroupID"] == null)
                            dr1["GroupID"] = 0;

                        if (dr1["GroupID"].ToString().Equals(oldID.ToString()))
                            dr1["GroupID"] = gf.OID;
                    }
                }
            }
            #endregion 处理OID 插入重复的问题。 Sys_GroupField ， Sys_MapAttr.

            int timeKeyIdx = 0;
            foreach (DataTable dt in ds.Tables)
            {
                timeKeyIdx++;
                timeKey = timeKey + timeKeyIdx.ToString();

                infoTable = "@导入:" + dt.TableName + " 出现异常。";
                switch (dt.TableName)
                {
                    case "WF_Flow": //模版文件。
                       
                        continue;
                    case "WF_NodeSubFlow": //延续子流程.
                        foreach (DataRow dr in dt.Rows)
                        {
                            SubFlow yg = new SubFlow();

                            //创建实体TS类，预防没有的数据导入.
                            GEEntity ge = new GEEntity("WF_NodeSubFlow");

                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "tonodeid":
                                    case "fk_node":
                                    case "nodeid":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_NodeSubFlow下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    case "fk_flow":
                                        val = fl.No;
                                        break;
                                    default:
                                        val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                        break;
                                }
                                yg.SetValByKey(dc.ColumnName, val);

                                //设置字段值.
                                ge.SetValByKey(dc.ColumnName, val);
                            }
                            yg.Insert(); //插入数据。
                            ge.Update();  //执行更新数据库补充TS新增的字段.
                        }
                        continue;
                    case "WF_FlowForm": //独立表单。 add 2013-12-03
                        //foreach (DataRow dr in dt.Rows)
                        //{
                        //    FlowForm cd = new FlowForm();
                        //    foreach (DataColumn dc in dt.Columns)
                        //    {
                        //        string val = dr[dc.ColumnName] as string;
                        //        if (val == null)
                        //            continue;
                        //        switch (dc.ColumnName.ToLower())
                        //        {
                        //            case "fk_flow":
                        //                val = fl.No;
                        //                break;
                        //            default:
                        //                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                        //                break;
                        //        }
                        //        cd.SetValByKey(dc.ColumnName, val);
                        //    }
                        //    cd.Insert();
                        //}
                        break;
                    case "WF_NodeForm": //节点表单权限。 2013-12-03
                        foreach (DataRow dr in dt.Rows)
                        {
                            NodeToolbar cd = new NodeToolbar();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "tonodeid":
                                    case "fk_node":
                                    case "nodeid":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_NodeForm下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    case "fk_flow":
                                        val = fl.No;
                                        break;
                                    default:
                                        val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                        break;
                                }
                                cd.SetValByKey(dc.ColumnName, val);
                            }
                            cd.Insert();
                        }
                        break;
                    case "Sys_FrmSln": //表单字段权限。 2013-12-03
                        foreach (DataRow dr in dt.Rows)
                        {
                            FrmField cd = new FrmField();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "tonodeid":
                                    case "fk_node":
                                    case "nodeid":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点Sys_FrmSln下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    case "fk_flow":
                                        val = fl.No;
                                        break;
                                    default:
                                        val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                        break;
                                }
                                cd.SetValByKey(dc.ColumnName, val);
                            }
                            cd.Insert();
                        }
                        break;
                    case "WF_NodeToolbar": //工具栏。
                        foreach (DataRow dr in dt.Rows)
                        {
                            NodeToolbar cd = new NodeToolbar();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "tonodeid":
                                    case "fk_node":
                                    case "nodeid":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_NodeToolbar下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    case "fk_flow":
                                        val = fl.No;
                                        break;
                                    default:
                                        val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                        break;
                                }
                                cd.SetValByKey(dc.ColumnName, val);
                            }
                            cd.OID = DBAccess.GenerOID();
                            cd.DirectInsert();
                        }
                        break;
                    case "Sys_FrmPrintTemplate":
                        foreach (DataRow dr in dt.Rows)
                        {
                            FrmPrintTemplate bt = new FrmPrintTemplate();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_flow":
                                        val = flowID.ToString();
                                        break;
                                    case "nodeid":
                                    case "fk_node":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点Sys_FrmPrintTemplate下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    default:
                                        break;
                                }
                                bt.SetValByKey(dc.ColumnName, val);
                            }

                            bt.MyPK = DBAccess.GenerGUID();

                            try
                            {
                                File.Copy(info.DirectoryName + "/" + bt.MyPK + ".rtf", BP.Difference.SystemConfig.PathOfWebApp + @"/DataUser/CyclostyleFile/" + bt.MyPK + ".rtf", true);
                            }
                            catch (Exception ex)
                            {
                                // infoErr += "@恢复单据模板时出现错误：" + ex.Message + ",有可能是您在复制流程模板时没有复制同目录下的单据模板文件。";
                            }
                            bt.Insert();
                        }
                        break;
                    case "WF_FrmNode": //Conds.xml。
                        DBAccess.RunSQL("DELETE FROM WF_FrmNode WHERE FK_Flow='" + fl.No + "'");
                        foreach (DataRow dr in dt.Rows)
                        {
                            FrmNode fn = new FrmNode();
                            fn.FlowNo = fl.No;
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_FrmNode下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    case "fk_flow":
                                        val = fl.No;
                                        break;
                                    case "fk_frm":
                                        if (fl.FlowDevModel == FlowDevModel.JiJian)
                                            val = "ND" + Int32.Parse(fl.No) + "01";
                                        break;
                                    default:
                                        break;
                                }
                                fn.SetValByKey(dc.ColumnName, val);
                            }
                            // 开始插入。
                            if (fn.FK_Frm.Equals(""))
                                continue;
                            fn.setMyPK(fn.FK_Frm + "_" + fn.NodeID+"_" + fl.No);
                            try
                            {
                                fn.Insert();
                            }catch(Exception ex)
                            {

                            }
                           
                        }
                        break;
                    case "WF_Cond": //Conds.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            Cond cd = new Cond();
                            cd.FlowNo = fl.No;
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "tonodeid":
                                    case "fk_node":
                                    case "nodeid":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_Cond下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    case "fk_flow":
                                        val = fl.No;
                                        break;
                                    default:
                                        val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                        break;
                                }
                                cd.SetValByKey(dc.ColumnName, val);
                            }
                            cd.RefPKVal=fl.No + "_" + cd.NodeID + "_" + cd.ToNodeID;
                            cd.FlowNo = fl.No;
                            cd.setMyPK(BP.DA.DBAccess.GenerGUID());
                            cd.DirectInsert();
                        }
                        break;
                    case "WF_NodeReturn"://可退回的节点。
                        foreach (DataRow dr in dt.Rows)
                        {
                            NodeReturn cd = new NodeReturn();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                    case "returnto":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_NodeReturn下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    default:
                                        break;
                                }
                                cd.SetValByKey(dc.ColumnName, val);
                            }

                            //开始插入。
                            try
                            {
                                cd.Insert();
                            }
                            catch
                            {
                                cd.Update();
                            }
                        }
                        break;
                    case "WF_Direction": //方向。
                        foreach (DataRow dr in dt.Rows)
                        {
                            Direction dir = new Direction();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "node":
                                    case "tonode":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_Direction下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    default:
                                        break;
                                }
                                dir.SetValByKey(dc.ColumnName, val);
                            }
                            dir.FlowNo = fl.No;
                            dir.Insert();
                        }
                        break;
                    case "WF_LabNote": //LabNotes.xml。
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            LabNote ln = new LabNote();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                ln.SetValByKey(dc.ColumnName, val);
                            }
                            idx++;
                            ln.FlowNo = fl.No;
                            ln.setMyPK(DBAccess.GenerGUID());
                            ln.DirectInsert();
                        }
                        break;
                    case "WF_NodeDept": //FAppSets.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            NodeDept dp = new NodeDept();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_NodeDept下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    default:
                                        break;
                                }
                                dp.SetValByKey(dc.ColumnName, val);
                            }
                            try
                            {
                                //如果部门不属于本组织的，就要删除.  
                                if (Glo.CCBPMRunModel != CCBPMRunModel.Single)
                                {
                                    BP.WF.Port.Admin2Group.Dept dept = new BP.WF.Port.Admin2Group.Dept(dp.DeptNo);
                                    if (dept.OrgNo.Equals(WebUser.OrgNo) == false)
                                        continue;
                                }
                                dp.Insert();
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        break;
                    case "WF_TemplateNode":
                        foreach (DataRow dr in dt.Rows)
                        {
                            BP.WF.Template.TemplateNode nd = new BP.WF.Template.TemplateNode();
                            nd.SetValByKey(NodeAttr.NodeID,int.Parse(flowID + dr[NodeAttr.NodeID].ToString().Substring(iOldFlowLength)));
                            nd.RetrieveFromDBSources();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "nodeid":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_Node下nodeid值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    case "fk_flow":
                                    case "fk_flowsort":
                                        continue;
                                    case "showsheets":
                                    case "histonds":
                                    case "groupstands": //去除不必要的替换
                                        string key = "@" + flowID;
                                        val = val.Replace(key, "@");
                                        break;
                                    default:
                                        break;
                                }
                                nd.SetValByKey(dc.ColumnName, val);
                            }
                            nd.SetValByKey(NodeAttr.FK_Flow,fl.No);
                            nd.DirectInsert();
                        }
                        break;
                    case "WF_Node": //导入节点信息.
                        foreach (DataRow dr in dt.Rows)
                        {
                            BP.WF.Template.NodeExt nd = new BP.WF.Template.NodeExt();
                            BP.WF.Template.NodeWorkCheck fwc = new NodeWorkCheck();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null) continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "nodeid":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_Node下nodeid值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    case "fk_flow":
                                    case "fk_flowsort":
                                        continue;
                                    case "showsheets":
                                    case "histonds":
                                    case "groupstands": //去除不必要的替换
                                        string key = "@" + flowID;
                                        val = val.Replace(key, "@");
                                        break;
                                    default:
                                        break;
                                }
                                nd.SetValByKey(dc.ColumnName, val);
                                fwc.SetValByKey(dc.ColumnName, val);
                            }

                            nd.FlowNo = fl.No;
                            nd.FlowName = fl.Name;
                            try
                            {

                                if (nd.EnMap.Attrs.Contains("OfficePrintEnable"))
                                {
                                    if (nd.GetValStringByKey("OfficePrintEnable") == "打印")
                                        nd.SetValByKey("OfficePrintEnable", 0);
                                }

                                try
                                {
                                    nd.DirectUpdate();
                                }
                                catch (Exception ex)
                                {
                                    // var i2 = 11; 
                                }

                                //把抄送的信息也导入里面去.
                                fwc.FWCVer = 1;  //设置为2019版本. 2018版是1个节点1个人,仅仅显示1个意见.
                                fwc.DirectUpdate();
                                DBAccess.RunSQL("DELETE FROM Sys_MapAttr WHERE FK_MapData='ND" + nd.NodeID + "'");
                            }
                            catch (Exception ex)
                            {
                                fwc.CheckPhysicsTable();

                                throw new Exception("@导入节点:FlowName:" + nd.FlowName + " nodeID: " + nd.NodeID + " , " + nd.Name + " 错误:" + ex.Message);
                            }

                        }

                        // 执行update 触发其他的业务逻辑。
                        foreach (DataRow dr in dt.Rows)
                        {
                            Node nd = new Node();
                            nd.NodeID = int.Parse(dr[NodeAttr.NodeID].ToString());
                            nd.RetrieveFromDBSources();
                            Node newNode = new Node();
                            newNode.Copy(nd);
                            newNode.FlowNo = fl.No;
                            //获取表单类别
                            string formType = dr[NodeAttr.FormType].ToString();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "nodefrmid":
                                        //绑定表单库的表单11不需要替换表单编号
                                        if (formType.Equals("11") == false)
                                        {
                                            int iFormTypeLength = iOldFlowLength + 2;
                                            if (val.Length > iFormTypeLength)
                                            {
                                                val = "ND" + flowID + val.Substring(iFormTypeLength);
                                            }
                                        }
                                        break;
                                    case "nodeid":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_Node下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    case "fk_flow":
                                    case "fk_flowsort":
                                        continue;
                                    case "showsheets":
                                    case "histonds":
                                    case "groupstands": //修复替换 
                                        string key = "@" + flowID;
                                        val = val.Replace(key, "@");
                                        break;
                                    default:
                                        break;
                                }
                                newNode.SetValByKey(dc.ColumnName, val);
                            }
                            newNode.FlowNo = fl.No;
                            newNode.FlowName = fl.Name;
                            newNode.DirectUpdate();
                        }

                        //更新 GroupStaNDs . HisToND 
                        string sql = "UPDATE WF_Node SET GroupStaNDs=Replace(GroupStaNDs,'@" + int.Parse(oldFlowNo) + "','@" + int.Parse(fl.No) + "'), HisToNDs=Replace(HisToNDs,'@" + int.Parse(oldFlowNo) + "','@" + int.Parse(fl.No) + "') WHERE FK_Flow='" + fl.No + "'";
                        DBAccess.RunSQL(sql);
                        break;
                    case "WF_NodeExt":
                        foreach (DataRow dr in dt.Rows)
                        {
                            BP.WF.Template.NodeExt nd = new BP.WF.Template.NodeExt();
                            nd.NodeID = int.Parse(flowID + dr[NodeAttr.NodeID].ToString().Substring(iOldFlowLength));
                            nd.RetrieveFromDBSources();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "nodeid":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_Node下nodeid值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    case "fk_flow":
                                    case "fk_flowsort":
                                        continue;
                                    case "showsheets":
                                    case "histonds":
                                    case "groupstands": //去除不必要的替换
                                        string key = "@" + flowID;
                                        val = val.Replace(key, "@");
                                        break;
                                    default:
                                        break;
                                }
                                nd.SetValByKey(dc.ColumnName, val);
                            }
                            nd.FlowNo = fl.No;
                            nd.DirectUpdate();
                        }
                        break;
                    case "WF_Selector":
                        foreach (DataRow dr in dt.Rows)
                        {
                            Selector selector = new Selector();
                            foreach (DataColumn dc in dt.Columns)
                            {

                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                if (dc.ColumnName.ToLower().Equals("nodeid"))
                                {
                                    if (val.Length < iOldFlowLength)
                                    {
                                        // 节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                        throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_Node下FK_Node值错误:" + val);
                                    }
                                    val = flowID + val.Substring(iOldFlowLength);
                                }

                                selector.SetValByKey(dc.ColumnName, val);
                            }
                            selector.DirectUpdate();
                        }
                        break;
                    case "WF_NodeStation": //FAppSets.xml。
                        DBAccess.RunSQL("DELETE FROM WF_NodeStation WHERE FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + fl.No + "')");
                        foreach (DataRow dr in dt.Rows)
                        {
                            NodeStation ns = new NodeStation();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_NodeStation下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    default:
                                        break;
                                }
                                ns.SetValByKey(dc.ColumnName, val);
                            }
                            ns.Insert();
                        }
                        break;
                    case "Sys_Enum": //RptEmps.xml。
                    case "Sys_Enums":
                        foreach (DataRow dr in dt.Rows)
                        {
                            SysEnum se = new Sys.SysEnum();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        break;
                                    default:
                                        break;
                                }
                                se.SetValByKey(dc.ColumnName, val);
                            }
                            // se.setMyPK(se.EnumKey + "_" + se.Lang + "_" + se.IntKey;

                            //设置orgNo.
                            se.OrgNo = WebUser.OrgNo;
                            se.ResetPK();

                            if (se.IsExits == true)
                                continue;
                            se.Insert();
                        }
                        break;
                    case "Sys_EnumMain": //RptEmps.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            SysEnumMain sem = new Sys.SysEnumMain();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                sem.SetValByKey(dc.ColumnName, val);
                            }
                            if (sem.IsExits)
                                continue;
                            sem.Insert();
                        }
                        break;
                    case "Sys_MapAttr": //RptEmps.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            MapAttr ma = new Sys.MapAttr();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_mapdata":
                                    case "keyofen":
                                    case "autofulldoc":
                                        if (val == null)
                                            continue;
                                        val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                        break;
                                    default:
                                        break;
                                }
                                ma.SetValByKey(dc.ColumnName, val);
                            }
                            bool b = ma.IsExit(BP.Sys.MapAttrAttr.FK_MapData, ma.FrmID,
                                      MapAttrAttr.KeyOfEn, ma.KeyOfEn);

                            ma.setMyPK(ma.FrmID + "_" + ma.KeyOfEn);
                            if (b == true)
                                ma.DirectUpdate();
                            else
                                ma.DirectInsert();
                        }
                        break;
                    case "Sys_MapData": //RptEmps.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            MapData md = new Sys.MapData();
                            string htmlCode = "";
                            foreach (DataColumn dc in dt.Columns)
                            {
                                if (dc.ColumnName == "HtmlTemplateFile")
                                {
                                    htmlCode = dr[dc.ColumnName] as string;
                                    continue;
                                }

                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + int.Parse(fl.No));
                                md.SetValByKey(dc.ColumnName, val);
                            }
                            md.Save();

                            //如果是开发者表单，赋值HtmlTemplateFile数据库的值并保存到DataUser下
                            if (md.HisFrmType == FrmType.Develop)
                            {
                                //string htmlCode = DBAccess.GetBigTextFromDB("Sys_MapData", "No", "ND" + oldFlowID, "HtmlTemplateFile");
                                if (DataType.IsNullOrEmpty(htmlCode) == false)
                                {
                                    htmlCode = htmlCode.Replace("ND" + oldFlowID, "ND" + int.Parse(fl.No));
                                    //保存到数据库，存储html文件
                                    //保存到DataUser/CCForm/HtmlTemplateFile/文件夹下
                                    string filePath = BP.Difference.SystemConfig.PathOfDataUser + "CCForm/HtmlTemplateFile/";
                                    if (Directory.Exists(filePath) == false)
                                        Directory.CreateDirectory(filePath);
                                    filePath = filePath + md.No + ".htm";
                                    //写入到html 中
                                    DataType.WriteFile(filePath, htmlCode);
                                    // HtmlTemplateFile 保存到数据库中
                                    DBAccess.SaveBigTextToDB(htmlCode, "Sys_MapData", "No", md.No, "HtmlTemplateFile");
                                }
                                else
                                {
                                    //如果htmlCode是空的需要删除当前节点的html文件
                                    string filePath = BP.Difference.SystemConfig.PathOfDataUser + "CCForm/HtmlTemplateFile/" + md.No + ".htm";
                                    if (File.Exists(filePath) == true)
                                        File.Delete(filePath);
                                    DBAccess.SaveBigTextToDB("", "Sys_MapData", "No", md.No, "HtmlTemplateFile");
                                }
                            }
                        }

                        break;
                    case "Sys_MapDtl": //RptEmps.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            MapDtl md = new Sys.MapDtl();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                md.SetValByKey(dc.ColumnName, val);
                            }
                            md.Save();
                            md.IntMapAttrs(); //初始化他的字段属性.
                        }
                        break;
                    case "Sys_MapExt":
                        foreach (DataRow dr in dt.Rows)
                        {
                            MapExt md = new Sys.MapExt();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                md.SetValByKey(dc.ColumnName, val);
                            }

                            //调整他的PK.
                            //md.InitPK();
                            md.Save(); //执行保存.
                        }
                        break;
                    case "Sys_FrmImg":
                        idx = 0;
                        timeKey = DateTime.Now.ToString("yyyyMMddHHmmss");
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmImg en = new FrmImg();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                en.SetValByKey(dc.ColumnName, val);
                            }

                            //设置主键.
                            en.setMyPK(en.FrmID + "_" + en.KeyOfEn);
                            en.Save(); //执行保存.
                        }
                        break;
                    case "Sys_FrmImgAth": //图片附件. 
                        idx = 0;
                        timeKey = DateTime.Now.ToString("yyyyMMddHHmmss");
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmImgAth en = new FrmImgAth();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                en.SetValByKey(dc.ColumnName, val);
                            }
                            en.setMyPK(en.FrmID + "_" + en.CtrlID);
                            en.Save();
                            //  en.setMyPK(Guid.NewGuid().ToString());
                        }
                        break;
                    case "Sys_FrmAttachment":
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmAttachment en = new FrmAttachment();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                en.SetValByKey(dc.ColumnName, val);
                            }

                            en.setMyPK(en.FrmID + "_" + en.NoOfObj);
                            en.Save();
                        }
                        break;
                    case "Sys_FrmEvent": //事件.
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmEvent en = new FrmEvent();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Equals("0") == false)
                                        {
                                            if (val.Length < iOldFlowLength)
                                            {
                                                //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                                throw new Exception("@导入模板名称：" + oldFlowName + "；节点Sys_FrmEvent下FK_Node值错误:" + val);
                                            }
                                            val = flowID + val.Substring(iOldFlowLength);
                                        }
                                        break;
                                    case "fk_flow":
                                        val = fl.No;
                                        break;
                                    default:
                                        val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                        break;
                                }

                                en.SetValByKey(dc.ColumnName, val);
                            }

                            //解决保存错误问题. 
                            en.setMyPK(DBAccess.GenerGUID());
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmRB": //Sys_FrmRB.
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmRB en = new FrmRB();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                en.SetValByKey(dc.ColumnName, val);
                            }
                            en.Insert();
                        }
                        break;
                    case "Sys_MapFrame":
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            MapFrame en = new MapFrame();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;


                                en.SetValByKey(dc.ColumnName, val.ToString().Replace("ND" + oldFlowID, "ND" + flowID));
                            }
                            en.DirectInsert();
                        }
                        break;
                    case "WF_NodeEmp": //FAppSets.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            NodeEmp ne = new NodeEmp();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_NodeEmp下FK_Node值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    default:
                                        break;
                                }
                                ne.SetValByKey(dc.ColumnName, val);
                            }
                            ne.Insert();
                        }
                        break;
                    case "WF_CCRole": //FAppSets.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            CCRole ccRole = new CCRole();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                String val = dr[dc.ColumnName] == null ? null : dr[dc.ColumnName].ToString();
                                if (val == null)
                                {
                                    continue;
                                }

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "nodeid":
                                        if (val.Length < iOldFlowLength)
                                        {
                                            //节点编号长度小于流程编号长度则为异常数据，异常数据不进行处理
                                            throw new Exception("@导入模板名称：" + oldFlowName + "；节点WF_CCRole下NodeID值错误:" + val);
                                        }
                                        val = flowID + val.Substring(iOldFlowLength);
                                        break;
                                    default:
                                        break;
                                }
                                ccRole.SetValByKey(dc.ColumnName, val);
                            }
                            ccRole.setMyPK(DBAccess.GenerGUID());
                            ccRole.Insert();
                        }
                        break;
                    case "Sys_GroupField":
                        //foreach (DataRow dr in dt.Rows)
                        //{
                        //    GroupField gf = new Sys.GroupField();
                        //    foreach (DataColumn dc in dt.Columns)
                        //    {
                        //        string val = dr[dc.ColumnName] as string;
                        //        if (val == null)
                        //            continue;
                        //        switch (dc.ColumnName.ToLower())
                        //        {
                        //            case "enname":
                        //            case "keyofen":
                        //            case "ctrlid": //升级傻瓜表单的时候,新增加的字段 add by zhoupeng 2016.11.21
                        //            case "frmid": //升级傻瓜表单的时候,新增加的字段 add by zhoupeng 2016.11.21
                        //                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                        //                break;
                        //            default:
                        //                break;
                        //        }
                        //        gf.SetValByKey(dc.ColumnName, val);
                        //    }
                        //    gf.InsertAsOID(gf.OID);
                        //}
                        break;
                 
                    default:
                        // infoErr += "Error:" + dt.TableName;
                        break;
                        //    throw new Exception("@unhandle named " + dt.TableName);
                }
            }

            #region 处理数据完整性。


            //DBAccess.RunSQL("UPDATE WF_Cond SET FK_Node=NodeID WHERE FK_Node=0");
            //DBAccess.RunSQL("UPDATE WF_Cond SET ToNodeID=NodeID WHERE ToNodeID=0");
            // DBAccess.RunSQL("DELETE FROM WF_Cond WHERE NodeID NOT IN (SELECT NodeID FROM WF_Node)");
            //  DBAccess.RunSQL("DELETE FROM WF_Cond WHERE ToNodeID NOT IN (SELECT NodeID FROM WF_Node) ");
            // DBAccess.RunSQL("DELETE FROM WF_Cond WHERE FK_Node NOT IN (SELECT NodeID FROM WF_Node) AND FK_Node > 0");

            //处理分组错误.
            Nodes nds = new Nodes(fl.No);
            foreach (Node nd in nds)
            {
                MapFrmFool cols = new MapFrmFool("ND" + nd.NodeID);
                cols.DoCheckFixFrmForUpdateVer();
            }
            #endregion

            //处理OrgNo 的导入问题.
            if (Glo.CCBPMRunModel != CCBPMRunModel.Single)
            {
                fl.RetrieveFromDBSources();
                fl.OrgNo = BP.Web.WebUser.OrgNo;
                fl.DirectUpdate();
            }


            if (infoErr == "")
            {
                infoTable = "";
                //写入日志.
                BP.Sys.Base.Glo.WriteUserLog("导入流程模板：" + fl.Name + " - " + fl.No);

                //创建track.
                Track.CreateOrRepairTrackTable(fl.No);
                return fl; // "完全成功。";
            }

            infoErr = "@执行期间出现如下非致命的错误：\t\r" + infoErr + "@ " + infoTable;
            throw new Exception(infoErr);
        }

        public static Node NewEtcNode(string flowNo, int x, int y, NodeType type)
        {
            Flow flow = new Flow(flowNo);

            Node nd = new Node();
            int idx = DBAccess.RunSQLReturnValInt("SELECT COUNT(NodeID) FROM WF_Node WHERE FK_Flow='" + flowNo + "'", 0);
            if (idx == 0)
                idx++;

            int nodeID = 0;
            //设置节点ID.
            while (true)
            {
                string strID = flowNo + idx.ToString().PadLeft(2, '0');
                nd.NodeID = int.Parse(strID);
                if (nd.IsExits == false)
                    break;
                idx++;
            }

            if (nd.NodeID > int.Parse(flowNo + "99"))
                throw new Exception("流程最大节点编号不可以超过100");

            if (type == NodeType.RouteNode)
                nd.Name = "条件" + nd.NodeID;

            if (type == NodeType.CCNode)
                nd.Name = "抄送" + nd.NodeID;

            if (type == NodeType.SubFlowNode)
                nd.Name = "子流程节点" + nd.NodeID;

            nd.FlowNo = flowNo;
            nd.HisNodeType = type; //抄送节点.
            nd.X = x;
            nd.Y = y;
            nd.Insert();
            return nd;
        }

        public static Node NewNode(string flowNo, int x, int y, string icon = null, int runModel = 0, int nodeType = 0)
        {
            Flow flow = new Flow(flowNo);

            NodeSimples nds = new NodeSimples();
            nds.Retrieve("FK_Flow", flowNo,"Step");

            Node nd = new Node();
            nd.FlowNo = flowNo;
            try
            {
                int nodeID = 0;
                int idx = 2; //从第2个开始.
                             //设置节点ID.
                while (true)
                {
                    string strID = "";
                    if (idx.ToString().Length == 3 || idx.ToString().Length == 2)
                    {
                        strID = flowNo + idx.ToString();
                        nd.NodeID = int.Parse(strID);
                    }
                    else
                    {
                        strID = flowNo + idx.ToString().PadLeft(2, '0');
                        nd.NodeID = int.Parse(strID);
                    }
                    if (nds.Contains("NodeID", nd.NodeID) == false)
                        break;
                    idx++;
                }
                //if (nd.NodeID > int.Parse(flowNo + "99"))
                //    throw new Exception("流程最大节点编号不可以超过100");
                nodeID = nd.NodeID;
                //增加了两个默认值值 . 2016.11.15. 目的是让创建的节点，就可以使用.
                nd.CondModel = DirCondModel.ByDDLSelected; //默认的发送方向.
                nd.HisDeliveryWay = DeliveryWay.BySelected;   //上一步发送人来选择.
                nd.FormType = NodeFormType.FoolForm; //设置为傻瓜表单.
                nd.Mark = nodeID.ToString(); //类别.
                nd.Insert();

                //如果是极简模式.
                if (flow.FlowDevModel == FlowDevModel.JiJian)
                {
                    nd.FormType = NodeFormType.FoolForm; //设置为傻瓜表单.
                    nd.NodeFrmID = "ND" + int.Parse(flow.No) + "01";
                    nd.FrmWorkCheckSta = FrmWorkCheckSta.Enable;
                    //  nd.DirectUpdate();

                    FrmNode fn = new FrmNode();
                    fn.FK_Frm = "ND" + int.Parse(flow.No) + "01";
                    fn.IsEnableFWC = FrmWorkCheckSta.Enable;
                    fn.NodeID = nd.NodeID;
                    fn.FlowNo = flowNo;
                    fn.FrmSln = FrmSln.Readonly;
                    fn.setMyPK(fn.FK_Frm + "_" + fn.NodeID + "_" + fn.FlowNo);
                    //执行保存.
                    fn.Save();
                }

                //如果是累加.
                if (flow.FlowDevModel == FlowDevModel.FoolTruck)
                {
                    nd.FormType = NodeFormType.FoolTruck; //设置为傻瓜表单.
                    nd.NodeFrmID = "ND" + nodeID;
                    nd.FrmWorkCheckSta = FrmWorkCheckSta.Disable;
                }

                //如果是绑定表单库的表单
                if (flow.FlowDevModel == FlowDevModel.RefOneFrmTree)
                {
                    nd.FormType = NodeFormType.RefOneFrmTree; //设置为傻瓜表单.
                    nd.NodeFrmID = flow.FrmUrl;
                    nd.FrmWorkCheckSta = FrmWorkCheckSta.Enable;
                    nd.DirectUpdate();
                    FrmNode fn = new FrmNode();
                    fn.FK_Frm = nd.NodeFrmID;
                    fn.IsEnableFWC = FrmWorkCheckSta.Enable;
                    fn.NodeID = nd.NodeID;
                    fn.FlowNo = flowNo;
                    fn.FrmSln = FrmSln.Readonly;
                    fn.setMyPK(fn.FK_Frm + "_" + fn.NodeID + "_" + fn.FlowNo);
                    //执行保存.
                    fn.Save();

                }
                //如果是Self类型的表单的类型
                if (flow.FlowDevModel == FlowDevModel.SDKFrmSelfPK || flow.FlowDevModel == FlowDevModel.SDKFrmWorkID)
                {
                    nd.HisFormType = NodeFormType.SDKForm;
                    nd.FormUrl = flow.FrmUrl;
                }

                //如果是Self类型的表单的类型
                if (flow.FlowDevModel == FlowDevModel.SelfFrm)
                {
                    nd.HisFormType = NodeFormType.SelfForm;
                    nd.FormUrl = flow.FrmUrl;
                }
                nd.FlowNo = flowNo;

                //为创建节点设置默认值   部分方法
                //string file = BP.Difference.SystemConfig.PathOfDataUser + "XML/DefaultNewNodeAttr.xml";
                //DataSet ds = new DataSet();
                //if (1 == 2 && System.IO.File.Exists(file) == true)
                //{
                //    ds.ReadXml(file);
                //    NodeExt ndExt = new NodeExt(nd.NodeID);
                //    DataTable dt = ds.Tables[0];
                //    foreach (DataColumn dc in dt.Columns)
                //    {
                //        nd.SetValByKey(dc.ColumnName, dt.Rows[0][dc.ColumnName]);
                //        ndExt.SetValByKey(dc.ColumnName, dt.Rows[0][dc.ColumnName]);
                //    }
                //    ndExt.FlowNo = flowNo;
                //    ndExt.NodeID = nodeID;
                //    ndExt.DirectUpdate();
                //}
                nd.FWCVer = 1; //设置为2019版本. 2018版是1个节点1个人,仅仅显示1个意见.
                nd.NodeID = nodeID;
                nd.HisDeliveryWay = DeliveryWay.BySelected;
                nd.X = x;
                nd.Y = y;
                nd.Icon = icon;
                nd.Step = idx;

                //节点类型.
                nd.HisNodeWorkType = NodeWorkType.Work;
                nd.Name = "New Node " + idx;
                nd.HisNodePosType = NodePosType.Mid;
                nd.FlowNo = flow.No;
                nd.FlowName = flow.Name;

                //设置审核意见的默认值.
                nd.SetValByKey(NodeWorkCheckAttr.FWCDefInfo,
                    BP.WF.Glo.DefVal_WF_Node_FWCDefInfo);

                //设置节点运行模式.
                nd.HisRunModel = (RunModel)runModel;
                //设置节点类型
                nd.HisNodeType = (NodeType)nodeType;
                nd.Update(); //执行更新. 
                nd.CreateMap(); //创建节点表单.

                //设置默认值。
                int state = 0;
                if (flow.FlowDevModel == FlowDevModel.JiJian)
                    state = 1;

                //设置审核组件的高度.
                DBAccess.RunSQL("UPDATE WF_Node SET FWC_H=300,FTC_H=300,SelectorModel=5,FWCSta=" + state + " WHERE NodeID=" + nd.NodeID + "");

                //创建默认的推送消息.
                CreatePushMsg(nd);

                //写入日志.
                BP.Sys.Base.Glo.WriteUserLog("创建节点：" + nd.Name + " - " + nd.NodeID);
                return nd;
            }
            catch (Exception ex)
            {
                DeleteNode(nd.NodeID);
                throw ex; 
            }
        }
        private static void CreatePushMsg(Node nd)
        {
            //删除已经存在的.
            BP.WF.Template.PushMsg pm = new BP.WF.Template.PushMsg();
            pm.Delete(PushMsgAttr.FK_Node, nd.NodeID);

            pm.FK_Event = EventListNode.SendSuccess;
            pm.NodeID = nd.NodeID;
            pm.FlowNo = nd.FlowNo;
            pm.SMSPushWay = 1;  // 发送短消息.
            pm.SMSPushModel = "Email";
            pm.setMyPK(DBAccess.GenerGUID());
            pm.Insert();

            pm.FK_Event = EventListNode.ReturnAfter;
            pm.NodeID = nd.NodeID;
            pm.FlowNo = nd.FlowNo;
            pm.SMSPushWay = 1;  // 发送短消息.
            pm.MailPushWay = 0; //不发送邮件消息.
            pm.setMyPK(DBAccess.GenerGUID());
            pm.Insert();
        }

        /// <summary>
        /// 检查当前人员是否有修改该流程模版的权限.
        /// </summary>
        /// <param name="flowNo"></param>
        /// <returns></returns>
        public static bool CheckPower(string flowNo)
        {
            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.GroupInc)
                return true;

            return true;

            if (BP.Web.WebUser.No.Equals("admin") == true)
                return true;

            string sql = "SELECT DesignerNo FROM WF_Flow WHERE No='" + flowNo + "'";
            string empNo = DBAccess.RunSQLReturnStringIsNull(sql, null);
            if (DataType.IsNullOrEmpty(empNo) == true)
                return true;

            if (empNo.Equals(BP.Web.WebUser.No) == false)
                throw new Exception("err@您没有权限对该流程修改.");

            return true;
        }
        /// <summary>
        /// 创建流程.
        /// </summary>
        /// <param name="flowSort"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(20); // 信号量
        public static Flow NewFlowByBPMN(string flowSort, string filePath)
        {
            #region 0. 读取文件.
            DataSet ds = new DataSet();
            ds.ReadXml(filePath);
            // 读取流程属性
            DataTable dtFlow = ds.Tables["process"];
            // 读取节点
            DataTable dtNode = ds.Tables["userTask"];
            // 读取结束节点
            DataTable dtEndNode = ds.Tables["endEvent"];
            //将结束节点放入节点中
            dtNode.ImportRow(dtEndNode.Rows[0]);
            //读取网关节点
            DataTable dtGateway = ds.Tables["exclusiveGateway"];
            // 读取方法条件
            DataTable dtDirs = ds.Tables["sequenceFlow"];
            // 图形数据
            DataTable dtShapes = ds.Tables["BPMNShape"];
            // 图形坐标
            DataTable dtPositions = ds.Tables["Bounds"];

            HttpContext ctx = HttpContextHelper.Current;

            //只支持单个流程的导入
            DataRow drFlow = dtFlow.Rows[0];

            string flowName = drFlow["name"].ToString(); //获得流程名称
            string flowMark = drFlow["id"].ToString(); //获得流程标记.
            Flow fl = new Flow();
            fl.FlowMark = flowMark;
            if (fl.RetrieveFromDBSources() == 1)
                throw new Exception("err@该流程[" + flowMark + "]已经导入,如果您要导入，请您修改模板的流程标记.");
            #endregion 检查完整性.

            #region 1. 创建空白的模板做准备..
            string sortNo = "001"; //放入的流程目录.
            string flowNo = BP.WF.Template.TemplateGlo.NewFlowTemplate(sortNo, flowName, DataStoreModel.ByCCFlow, null, null);

            fl.No = flowNo;
            fl.RetrieveFromDBSources();
            fl.FlowMark = flowMark; //更新标记.
            fl.FlowSortNo = flowSort; //更新流程目录
            fl.Update();

            //删除第2个节点信息.
            Node nd = new Node();
            nd.NodeID = int.Parse(flowNo + "02");
            nd.Delete(); //删除节点.
            BP.WF.Template.Directions dir = new Directions();
            dir.Delete(DirectionAttr.FK_Flow, flowNo);
            //Node nd = new Node();
            //string flowNo = "028";
            #endregion 0. 检查完整性.

            #region 2. 生成节点.
            // Step1: 遍历节点, 并生成节点.
            // ccflow 节点是以01开头的
            ConcurrentDictionary<string, int> relations = new ConcurrentDictionary<string, int>(); // 保存节点id 对应 关系
            ConcurrentDictionary<int, string> nodeUserTaskID = new ConcurrentDictionary<int, string>(); // 保存节点id与userTaskID 对应 关系
            ConcurrentBag<Node> flowNodes = new ConcurrentBag<Node>();
            CountdownEvent cdEvent = new CountdownEvent(dtNode.Rows.Count);
            for (int i = 0; i < dtNode.Rows.Count; i++)
            {
                int temp = i + 1;
                string nodeSuffix = "";
                if (temp < 10)
                {
                    nodeSuffix = "0" + temp;
                }
                else
                {
                    nodeSuffix = temp.ToString();
                }
                DataRow dr = dtNode.Rows[i];
                //ThreadPool.QueueUserWorkItem(_ =>
                //{
                //    _semaphore.Wait();
                //    try
                //    {
                HttpContext.Current = ctx;
                #region 获得节点信息.
                string userTaskID = dr["id"] as string;
                if (i == 0)
                    nd = new Node(int.Parse(flowNo + "01")); //开始节点.
                else
                    nd = TemplateGlo.NewNode(flowNo, 100, 100);
                //nd = new Node(int.Parse(flowNo + nodeSuffix));
                // 找到图形信息
                int shapeID = -1;
                foreach (DataRow row in dtShapes.Rows)
                {
                    string bpmnElement = (string)row["bpmnElement"];
                    if (bpmnElement == userTaskID)
                    {
                        shapeID = (int)row["BPMNShape_Id"];
                        break;
                    }
                }
                if (shapeID == -1)
                {
                    BP.DA.Log.DebugWriteError("解析BPMN出现异常数据, 存在多个节点为[" + userTaskID + "]的数据");
                    continue;
                }
                bool hasPostion = false;
                // 找到坐标
                foreach (DataRow row in dtPositions.Rows)
                {
                    int bpmnShapeId = (int)row["BPMNShape_Id"];
                    if (bpmnShapeId == shapeID)
                    {
                        nd.X = (int)float.Parse(row["x"] as string);
                        nd.Y = (int)float.Parse(row["y"] as string);
                        hasPostion = true;
                        break;
                    }
                }
                if (!hasPostion)
                {
                    nd.X = 100;
                    nd.Y = 100;
                }
                List<string> lines = new List<string>();
                // 找到所有以他为起点的连接线
                foreach (DataRow row in dtDirs.Rows)
                {
                    string sourceRef = (string)row["sourceRef"];
                    if (sourceRef == userTaskID)
                    {
                        lines.Add(row["targetRef"] as string);
                    }
                }
                nd.Name = dr["name"] as string;
                nd.Mark = String.Join(",", lines.ToArray());
                nd.Update(); //更新节点信息.

                relations.TryAdd(userTaskID, nd.NodeID);  // 保存关系
                nodeUserTaskID.TryAdd(nd.NodeID, userTaskID);  // 保存关系
                flowNodes.Add(nd);  // 保存节点
                #endregion 获得节点信息.
                //    }
                //    finally
                //    {
                //        cdEvent.Signal();
                //        _semaphore.Release();
                //    }
                //});
            }
            //cdEvent.Wait();
            #endregion 2. 生成节点.

            #region 3. 生成网关节点
            for (int i = 0; i < dtGateway.Rows.Count; i++)
            {
                DataRow dr = dtGateway.Rows[i];
                string gatewayID = dr["id"] as string;
                //创建网关节点
                nd = TemplateGlo.NewNode(flowNo, 100, 100, null, 0, 1);
                // 找到图形信息
                int shapeID = -1;
                foreach (DataRow row in dtShapes.Rows)
                {
                    string bpmnElement = (string)row["bpmnElement"];
                    if (bpmnElement == gatewayID)
                    {
                        shapeID = (int)row["BPMNShape_Id"];
                        break;
                    }
                }
                if (shapeID == -1)
                {
                    BP.DA.Log.DebugWriteError("解析BPMN出现异常数据, 存在多个节点为[" + gatewayID + "]的数据");
                    continue;
                }
                bool hasPostion = false;
                // 找到坐标
                foreach (DataRow row in dtPositions.Rows)
                {
                    int bpmnShapeId = (int)row["BPMNShape_Id"];
                    if (bpmnShapeId == shapeID)
                    {
                        nd.X = (int)float.Parse(row["x"] as string);
                        nd.Y = (int)float.Parse(row["y"] as string);
                        hasPostion = true;
                        break;
                    }
                }
                if (!hasPostion)
                {
                    nd.X = 100;
                    nd.Y = 100;
                }
                //保存连接线
                List<string> lines = new List<string>();
                //保存连接线的描述
                List<string> docs = new List<string>();
                // 找到所有以他为起点的连接线
                foreach (DataRow row in dtDirs.Rows)
                {
                    string sourceRef = (string)row["sourceRef"];
                    if (sourceRef == gatewayID)
                    {
                        lines.Add(row["targetRef"] as string);
                        docs.Add(row["name"] == null ? "" : row["name"] as string);
                    }
                }
                nd.Name = dr["name"] as string;
                nd.Mark = String.Join(",", lines.ToArray());
                nd.Doc = String.Join(",", docs.ToArray());
                nd.Update(); //更新节点信息.

                relations.TryAdd(gatewayID, nd.NodeID);  // 保存关系
                nodeUserTaskID.TryAdd(nd.NodeID, gatewayID);  // 保存关系
                flowNodes.Add(nd);  // 保存节点
            }
            #endregion 3. 生成网关节点

            #region 4. 生成链接线.
            // 插入连接线，经过上面的流程后才知道对应关系
            foreach (Node node in flowNodes)
            {
                try
                {
                    string[] toUserTasks = node.Mark.Split(',');
                    node.Mark = nodeUserTaskID[node.NodeID];
                    string[] doc = new string[0];
                    //判断如果是路由节点
                    if (node.HisNodeType == NodeType.RouteNode)
                    {
                        doc = node.Doc.Split(',');
                        node.Doc = "";//清空临时保存的描述
                    }
                    node.Update();
                    for (int i = 0; i < toUserTasks.Length; i++)
                    {
                        string toUserTask = toUserTasks[i];
                        int toNodeId = relations[toUserTask];
                        //生成方向.
                        Direction mydir = new Direction();
                        mydir.FlowNo = flowNo;
                        mydir.Node = node.NodeID;
                        mydir.ToNode = toNodeId;
                        //判断如果有描述就添加
                        if (doc.Length > 0)
                        {
                            mydir.Des = doc[i];
                        }
                        mydir.Insert(); //自动生成主键.
                    }

                }
                catch (Exception ex)
                {
                    BP.DA.Log.DebugWriteError("解析BPMN-创建连接线失败：nodeId=" + node.NodeID + ", Mark = " + node.Mark);
                    continue;
                }

            }
            // Step2: 遍历节点, 生成连接线.
            //Nodes nds = new Nodes();
            //nds.RetrieveAll();
            //foreach (Node node in nds)
            //{
            //    string userTaskID = node.Mark; //当前节点,

            //    //获得到达节点的ID.
            //    string[] toTaskIDs = "ssss".Split();
            //    foreach (string taskID in toTaskIDs)
            //    {
            //        //查询出来，到达的节点ID.
            //        Node tond = new Node();
            //        tond.Retrieve(NodeAttr.Mark, taskID);

            //        //生成方向.
            //        Direction mydir = new Direction();
            //        mydir.FlowNo = flowNo;
            //        mydir.Node = node.NodeID;
            //        mydir.ToNode = nd.NodeID;
            //        mydir.Insert(); //自动生成主键.
            //    }

            //    //检查方向条件.
            //    string[] conds = "ssss".Split();
            //    foreach (string cond in conds)
            //    {
            //        Cond mycond = new Cond();
            //    }
            //}
            #endregion 4. 生成链接线.

            return fl;
        }

        /// <summary>
        /// 创建一个流程模版
        /// </summary>
        /// <param name="flowSort">流程类别</param>
        /// <param name="flowName">名称</param>
        /// <param name="dsm">存储方式</param>
        /// <param name="ptable">物理量</param>
        /// <param name="flowMark">标记</param>
        /// <returns>创建的流程编号</returns>
        public static string NewFlowTemplate(string flowSort, string flowName, BP.WF.Template.DataStoreModel dsm,
        string ptable, string flowMark)
        {
            //定义一个变量.
            Flow flow = new Flow();
            try
            {
                //检查参数的完整性.
                if (DataType.IsNullOrEmpty(ptable) == false && ptable.Length >= 1)
                {
                    string c = ptable.Substring(0, 1);
                    if (DataType.IsNumStr(c) == true)
                        throw new Exception("@非法的流程数据表(" + ptable + "),它会导致ccflow不能创建该表.");
                }

                flow.PTable = ptable;
                flow.FlowSortNo = flowSort;
                flow.FlowMark = flowMark;

                if (DataType.IsNullOrEmpty(flowMark) == false)
                {
                    if (flow.IsExit(FlowAttr.FlowMark, flowMark))
                        throw new Exception("@该流程标示:" + flowMark + "已经存在于系统中.");
                }

                /*给初始值*/
                //this.Paras = "@StartNodeX=10@StartNodeY=15@EndNodeX=40@EndNodeY=10";
                flow.Paras = "@StartNodeX=200@StartNodeY=50@EndNodeX=200@EndNodeY=350";

                flow.No = flow.GenerNewNoByKey(FlowAttr.No);
                flow.Name = flowName;
                if (DataType.IsNullOrEmpty(flow.Name))
                    flow.Name = "新建流程" + flow.No; //新建流程.

                //if (flow.IsExits == true)
                //    throw new Exception("err@系统出现自动生成的流程编号重复.");

                if (Glo.CCBPMRunModel != CCBPMRunModel.Single)
                    flow.OrgNo = WebUser.OrgNo; //隶属组织 

                flow.PTable = "ND" + int.Parse(flow.No) + "Rpt";

                // 设置创建人，创建日期.
                flow.SetValByKey(FlowAttr.CreateDate, DataType.CurrentDateTime);
                flow.SetValByKey(FlowAttr.Creater, BP.Web.WebUser.No);
                // flow.SetValByKey("Icon", "icon-people");
                flow.SetValByKey("ICON", "icon-people"); //图标.

                //flow.TitleRole
                flow.Insert();

                ////如果是集团模式下.
                //if (Glo.CCBPMRunModel == CCBPMRunModel.GroupInc)
                //{
                //    // 记录创建人.
                //    FlowExt fe = new FlowExt(flow.No);
                //    fe.DesignerNo = BP.Web.WebUser.No;
                //    fe.DesignerName = BP.Web.WebUser.Name;
                //    fe.DesignTime = DataType.CurrentDateTime;
                //    fe.DirectUpdate();
                //}

                BP.WF.Node nd = new BP.WF.Node();
                nd.NodeID = int.Parse(flow.No + "01");
                nd.Name = "Start Node";//  "开始节点"; 
                nd.Step = 1;
                nd.FlowNo = flow.No;
                nd.FlowName = flow.Name;
                nd.HisNodePosType = NodePosType.Start;
                nd.HisNodeWorkType = NodeWorkType.StartWork;
                nd.X = 200;
                nd.Y = 150;
                nd.NodePosType = NodePosType.Start;
                nd.HisReturnRole = ReturnRole.CanNotReturn; //不能退回.
                nd.Icon = "前台";

                //增加了两个默认值值 . 2016.11.15. 目的是让创建的节点，就可以使用.
                nd.CondModel = DirCondModel.ByDDLSelected; //默认的发送方向.
                nd.HisDeliveryWay = DeliveryWay.BySelected; //上一步发送人来选择.
                nd.FormType = NodeFormType.FoolForm; //设置为傻瓜表单.

                //如果是集团模式.   
                if (Glo.CCBPMRunModel == CCBPMRunModel.GroupInc)
                {
                    if (DataType.IsNullOrEmpty(WebUser.OrgNo) == true)
                        throw new Exception("err@登录信息丢失了组织信息,请重新登录.");

                    nd.HisDeliveryWay = DeliveryWay.BySelectedOrgs;

                    //把本组织加入进去.
                    FlowOrg fo = new FlowOrg();
                    string mypk = WebUser.OrgNo + "_" + nd.FlowNo;
                    fo.Delete("MyPK", mypk);
                    fo.SetValByKey("MyPK", mypk);
                    fo.FlowNo = nd.FlowNo;
                    fo.OrgNo = WebUser.OrgNo;
                    fo.Insert();
                }

                nd.Insert();
                nd.CreateMap();

                //为开始节点增加一个删除按钮.
                string sql = "UPDATE WF_Node SET DelEnable=1 WHERE NodeID=" + nd.NodeID;
                DBAccess.RunSQL(sql);

                //nd.HisWork.CheckPhysicsTable();  去掉，检查的时候会执行.
                CreatePushMsg(nd);

                //通用的人员选择器.
                BP.WF.Template.Selector select = new Selector(nd.NodeID);
                select.SelectorModel = SelectorModel.GenerUserSelecter;
                select.Update();

                nd = new BP.WF.Node();

                //为创建节点设置默认值 
                string fileNewNode = BP.Difference.SystemConfig.PathOfDataUser + "XML/DefaultNewNodeAttr.xml";
                if (System.IO.File.Exists(fileNewNode) == true && 1 == 2)
                {
                    DataSet myds = new DataSet();
                    myds.ReadXml(fileNewNode);
                    DataTable dt = myds.Tables[0];
                    foreach (DataColumn dc in dt.Columns)
                    {
                        nd.SetValByKey(dc.ColumnName, dt.Rows[0][dc.ColumnName]);
                    }
                }
                else
                {
                    nd.HisNodePosType = NodePosType.Mid;
                    nd.HisNodeWorkType = NodeWorkType.Work;
                    nd.X = 200;
                    nd.Y = 250;
                    nd.Icon = "审核";
                    nd.NodePosType = NodePosType.End;

                    //增加了两个默认值值 . 2016.11.15. 目的是让创建的节点，就可以使用.
                    nd.CondModel = DirCondModel.ByDDLSelected; //默认的发送方向.
                    nd.HisDeliveryWay = DeliveryWay.BySelected; //上一步发送人来选择.
                    nd.FormType = NodeFormType.FoolForm; //设置为傻瓜表单.
                }

                nd.NodeID = int.Parse(flow.No + "02");
                nd.Name = "Node 2"; // "结束节点";
                nd.Step = 2;
                nd.FlowNo = flow.No;
                nd.FlowName = flow.Name;
                nd.HisDeliveryWay = DeliveryWay.BySelected; //上一步发送人来选择.
                nd.FormType = NodeFormType.FoolForm; //设置为傻瓜表单.
                nd.CondModel = DirCondModel.ByDDLSelected;

                nd.X = 200;
                nd.Y = 250;

                //设置审核意见的默认值.
                nd.SetValByKey(NodeWorkCheckAttr.FWCDefInfo, BP.WF.Glo.DefVal_WF_Node_FWCDefInfo);

                nd.Insert();
                nd.CreateMap();
                //nd.HisWork.CheckPhysicsTable(); //去掉，检查的时候会执行.
                CreatePushMsg(nd);

                //通用的人员选择器.
                select = new Selector(nd.NodeID);
                select.SelectorModel = SelectorModel.GenerUserSelecter;
                select.Update();

                BP.Sys.MapData md = new BP.Sys.MapData();
                md.No = "ND" + int.Parse(flow.No) + "Rpt";
                md.Name = flow.Name;
                md.Save();

                // 装载模版.
                string file = BP.Difference.SystemConfig.PathOfDataUser + "XML/TempleteSheetOfStartNode.xml";
                if (System.IO.File.Exists(file) == true && 1 == 2)
                {
                    //throw new Exception("@开始节点表单模版丢失" + file); 
                    /*如果存在开始节点表单模版*/
                    DataSet ds = new DataSet();
                    ds.ReadXml(file);

                    string nodeID = "ND" + int.Parse(flow.No + "01");
                    BP.Sys.MapData.ImpMapData(nodeID, ds);
                }

                //加载默认字段.
                if (1 == 1)
                {
                    string frmID = "ND" + int.Parse(flow.No + "01");

                    //创建表单ID。
                    int groupID = DBAccess.RunSQLReturnValInt("SELECT MAX(GroupID) FROM Sys_MapAttr WHERE FK_MapData='" + frmID + "'", 0);
                    if (groupID == 0)
                    {
                        GroupField gf = new GroupField();
                        gf.Lab = "基本信息";
                        gf.FrmID = frmID;
                        gf.Insert();
                        groupID = gf.OID;
                    }

                    MapAttr attr = new MapAttr();
                    attr.setMyPK(frmID + "_SQR");
                    attr.setName("申请人");
                    attr.setKeyOfEn("SQR");
                    attr.setDefValReal("@WebUser.Name");
                    attr.setUIVisible(true);
                    attr.setUIIsEnable(false);
                    attr.FrmID =frmID;
                    attr.ColSpan = 1;
                    attr.setGroupID(groupID);
                    attr.Insert();

                    attr = new MapAttr();
                    attr.setMyPK(frmID + "_SQDT");
                    attr.setName("申请日期");
                    attr.setKeyOfEn("SQRQ");
                    attr.setDefValReal("@RDT");
                    attr.setUIVisible(true);
                    attr.setUIIsEnable(false);
                    attr.FrmID =frmID;
                    attr.MyDataType = DataType.AppDateTime;
                    attr.ColSpan = 1;
                    attr.setGroupID(groupID);
                    attr.Insert();

                    attr = new MapAttr();
                    attr.setMyPK(frmID + "_SQDept");
                    attr.setName("申请人部门");
                    attr.setKeyOfEn("SQDept");
                    attr.setDefValReal("@WebUser.FK_DeptName");
                    attr.setUIVisible(true);
                    attr.setUIIsEnable(false);
                    attr.FrmID =frmID;
                    attr.ColSpan = 3;
                    attr.setGroupID(groupID);
                    attr.Insert();
                }

                //创建track.
                Track.CreateOrRepairTrackTable(flow.No);

            }
            catch (Exception ex)
            {
                ///删除垃圾数据.
                flow.DoDelete();
                //提示错误.
                throw new Exception("err@创建流程错误:" + ex.Message);
            }

            //FlowExt flowExt = new FlowExt(flow.No);
            //flowExt.DesignerNo = BP.Web.WebUser.No;
            //flowExt.DesignerName = BP.Web.WebUser.Name;
            //flowExt.DesignTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //flowExt.DirectSave();

            //创建连线
            Direction drToNode = new Direction();
            drToNode.FlowNo = flow.No;
            drToNode.Node = int.Parse(int.Parse(flow.No) + "01");
            drToNode.ToNode = int.Parse(int.Parse(flow.No) + "02");
            drToNode.Save();

            //增加方向.
            Node mynd = new Node(drToNode.Node);
            mynd.HisToNDs = drToNode.ToNode.ToString();
            mynd.Update();

            //设置流程的默认值.
            foreach (string key in SystemConfig.AppSettings.AllKeys)
            {
                if (key.Contains("NewFlowDefVal") == false)
                    continue;

                string val = BP.Difference.SystemConfig.AppSettings[key];
                //设置值.
                flow.SetValByKey(key.Replace("NewFlowDefVal_", ""), val);
            }
            //执行一次流程检查, 为了节省效率，把检查去掉了.
            flow.DoCheck();

            //写入日志.
            BP.Sys.Base.Glo.WriteUserLog("创建流程：" + flow.Name + " - " + flow.No);

            return flow.No;
        }
        /// <summary>
        /// 删除节点.
        /// </summary>
        /// <param name="nodeid"></param>
        public static void DeleteNode(int nodeid)
        {
            BP.WF.Node nd = new BP.WF.Node(nodeid);
            nd.Delete();
        }
    }
}
