using System.Data;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.En;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Admin_AttrNode_FrmSln : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_AttrNode_FrmSln()
        {

        }
        /// <summary>
        /// 设置该流程的所有节点都是用该方案。
        /// </summary>
        /// <returns></returns>
        public string RefOneFrmTree_SetAllNodeFrmUseThisSln()
        {
            string nodeID = GetRequestVal("FK_Node");
            Node currNode = new Node(this.NodeID);
            string flowNo = currNode.FlowNo;
            Nodes nds = new Nodes();
            nds.Retrieve("FK_Flow", flowNo);

            //求出来组件s.
            MapAttrs attrOfCommpents = new MapAttrs();
            QueryObject qo = new QueryObject(attrOfCommpents);
            qo.AddWhere(MapAttrAttr.FK_MapData, currNode.NodeFrmID);
            qo.addAnd();
            qo.AddWhere(MapAttrAttr.UIContralType, ">=", 6);
            qo.DoQuery();

         //   attrOfCommpents.Retrieve(MapAttrAttr.FK_MapData,
           // currNode.)

            for (int i = 0; i < nds.Count; i++)
            {

                Node jsNode = nds[i] as Node;
                if (jsNode.NodeID == currNode.NodeID)
                    continue;

                //修改表单属性
                jsNode.FormType = currNode.FormType;
               
                jsNode.NodeFrmID = currNode.NodeFrmID;
                jsNode.Update();

                //节点表单属性
                //先删除掉已有的，避免换绑时出现垃圾数据.
                FrmNodes ens = new FrmNodes();
                ens.Retrieve("FK_Node", jsNode.NodeID);

                //是不是该frmNode已经存在？
                var isHave = false;
                for (int idx = 0; idx < ens.Count; idx++)
                {
                    FrmNode en = ens[idx] as FrmNode;
                    if (en.FK_Frm != currNode.NodeFrmID)
                    {
                        FrmNode Frm = new FrmNode(en.MyPK);
                        Frm.Delete();
                        continue;
                    }
                    isHave = true;
                }
                if (isHave == true)
                    continue; //已经存在就不处理.

                FrmNode frmNode = new FrmNode();
                frmNode.setMyPK(jsNode.NodeFrmID + "_" + jsNode.NodeID + "_" + jsNode.FlowNo);
                frmNode.NodeID = jsNode.NodeID;
                frmNode.FlowNo = jsNode.FlowNo;
                frmNode.FK_Frm = jsNode.NodeFrmID;

                //判断是否为开始节点
                string nodeID1 = jsNode.NodeID.ToString();
                if (nodeID1.Substring(nodeID1.Length - 2) == "01")
                {
                    frmNode.FrmSln = FrmSln.Default; //默认方案
                }
                else
                {
                    frmNode.FrmSln = FrmSln.Readonly; //只读方案
                }
                frmNode.Insert();

                //设置组件都是可用的.
                BP.WF.Template.FrmField ff = new FrmField();
                foreach (MapAttr attr in attrOfCommpents)
                {
                    ff.setUIVisible(true);
                    ff.setKeyOfEn(attr.KeyOfEn);
                    ff.FlowNo = currNode.FlowNo;
                    ff.NodeID = jsNode.NodeID;
                    ff.FrmID =jsNode.NodeFrmID; //表单ID.
                    ff.setMyPK(ff.FrmID + "_" + ff.NodeID + "_" + ff.KeyOfEn);
                    if (ff.IsExits == false)
                        ff.Insert();
                }
            }

            return "执行成功.";
        }
        /// <summary>
        /// 获得下拉框的值.
        /// </summary>
        /// <returns></returns>
        public string BatchEditSln_InitDDLData()
        {
            DataSet ds = new DataSet();

            SysEnums ses = new SysEnums("FrmSln");
            ds.Tables.Add(ses.ToDataTableField("FrmSln"));

            SysEnums se1s = new SysEnums("FWCSta");
            ds.Tables.Add(se1s.ToDataTableField("FWCSta"));

            //签字类型.
            SysEnums myses = new SysEnums("SigantureEnabel");
            ds.Tables.Add(myses.ToDataTableField("SigantureEnabel"));

            string sql = Glo.SQLOfCheckField.Replace("@FK_Frm", this.FrmID);
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dt.Columns[0].ColumnName = "No";
                dt.Columns[1].ColumnName = "Name";
            }
                
            dt.TableName = "CheckFields";
            ds.Tables.Add(dt);
            return BP.Tools.Json.ToJson(ds);
        }

        /// <summary>
        /// 返回
        /// </summary>
        /// <returns></returns>
        public string RefOneFrmTreeFrms_Init()
        {
            string sql = "";
            string key = GetRequestVal("KeyWord");//查询的关键字
            //单机模式下
            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
            {
                sql += "SELECT  b.NAME AS SortName, a.no AS \"No\", A.name AS \"Name\",";
                sql += "A.PTable,";
                sql += "A.OrgNo ";
                sql += "FROM ";
                sql += "Sys_MapData A, ";
                sql += "Sys_FormTree B ";
                sql += " WHERE ";
                sql += " A.FK_FormTree = B.NO ";
                //sql += " AND B.OrgNo = '" + WebUser.OrgNo + "'";
                if (DataType.IsNullOrEmpty(key) == false)
                    sql += " AND A.Name like '%" + key + "%'";
                sql += "ORDER BY B.IDX,A.IDX";

            }

            // 云服务器环境下
            if (Glo.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                sql += "SELECT  b.NAME AS SortName, a.no AS \"No\", A.name AS \"Name\", ";
                sql += "A.PTable, ";
                sql += "A.OrgNo ";
                sql += "FROM ";
                sql += "Sys_MapData A, ";
                sql += "Sys_FormTree B ";
                sql += " WHERE ";
                sql += " A.FK_FormTree = B.No ";
                sql += " AND B.OrgNo = '" + WebUser.OrgNo + "' ";
                if (DataType.IsNullOrEmpty(key) == false)
                    sql += " AND A.Name like '%" + key + "%'";
                sql += "ORDER BY B.IDX,A.IDX";
            }

            //集团模式下
            if (Glo.CCBPMRunModel == CCBPMRunModel.GroupInc)
            {
                sql += " SELECT  b.NAME AS SortName, a.no AS \"No\", A.name AS \"Name\",";
                sql += "A.PTable,";
                sql += "A.OrgNo, '"+BP.Web.WebUser.OrgName+"' as OrgName ";
                sql += "FROM ";
                sql += "Sys_MapData A, ";
                sql += "Sys_FormTree B, ";
                sql += "Port_Org C ";
                sql += " WHERE ";
                sql += " A.FK_FormTree = B.NO ";
                sql += " AND B.OrgNo = '" + WebUser.OrgNo + "' ";
                sql += " AND C.No =B.OrgNo ";
                if (DataType.IsNullOrEmpty(key) == false)
                    sql += " AND A.Name like '%" + key + "%'";

                sql += " UNION  ";

                sql += " SELECT  '- 共享 -' AS SortName, a.no AS \"No\", A.name AS \"Name\", ";
                sql += " A.PTable, A.OrgNo, '其他组织' as OrgName ";
                sql += " FROM ";
                sql += " Sys_MapData A,  WF_FrmOrg B, Port_Org C ";
                sql += " WHERE ";
                sql += "  A.No = B.FrmID  AND B.OrgNo=C.No ";
                sql += "  AND B.OrgNo = '" + WebUser.OrgNo + "' ";
                if (DataType.IsNullOrEmpty(key) == false)
                    sql += " AND A.Name like '%" + key + "%'";
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            #warning 需要判断不同的数据库类型
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["SORTNAME"].ColumnName = "SortName";
                dt.Columns["NO"].ColumnName = "No";
                dt.Columns["NAME"].ColumnName = "Name";
                dt.Columns["PTABLE"].ColumnName = "PTable";
                dt.Columns["ORGNO"].ColumnName = "OrgNo";
            }
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["sortname"].ColumnName = "SortName";
                dt.Columns["no"].ColumnName = "No";
                dt.Columns["name"].ColumnName = "Name";
                dt.Columns["ptable"].ColumnName = "PTable";
                dt.Columns["orgno"].ColumnName = "OrgNo";
            }

            return BP.Tools.Json.ToJson(dt);
        }

    }
}
