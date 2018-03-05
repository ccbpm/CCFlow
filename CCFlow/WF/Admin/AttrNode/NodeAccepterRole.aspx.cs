using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.WF.Template;
using BP.WF;
using BP.DA;
using System.Data;

namespace CCFlow.WF.Admin.FlowNodeAttr
{
    public partial class NodeAccepterRole : BP.Web.WebPage
    {
        #region 参数
        public int NodeID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FK_Node"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 页面消息
        /// </summary>
        public string PageMessage
        {
            get;
            set;
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (IsPostBack==false)
            {
                //加载 06.按上一节点表单指定的字段值作为本步骤的接受人
                bindDDL_5();

                //加载 09.与指定节点处理人相同 
                bindCBL();

                BP.WF.Node nd = new Node(this.NodeID);

                BP.WF.Template.SQLTemplates sqlens=new SQLTemplates();
                sqlens.Retrieve(SQLTemplateAttr.SQLType, 1);
                BP.Web.Controls.Glo.DDL_BindEns(this.DDL_SQLTemplate, sqlens, nd.DeliveryParas);

                //是否可以分配工作？
                this.CB_IsSSS.Checked = nd.IsTask;

                //是否启用自动记忆功能
                this.CB_IsRememme.Checked = nd.IsRememberMe;

                //本节点接收人不允许包含上一步发送人
                this.CB_IsExpSender.Checked = nd.IsExpSender;

                //节点访问规则
                switch (nd.HisDeliveryWay)
                {
                    case DeliveryWay.ByStation:  //按照岗位.
                        this.RB_ByStation.Checked = true;
                        break;
                    case DeliveryWay.ByDept: //按部门.
                        this.RB_ByDept.Checked = true;
                        break;
                    case DeliveryWay.BySQL: //按SQL
                        this.RB_BySQL.Checked = true;
                        this.TB_BySQL.Text = nd.DeliveryParas; // dt.Rows[0]["DeliveryParas"].ToString();
                        break;
                    case DeliveryWay.BySQLTemplate: //按SQLTemplate
                        this.RB_BySQLTemplate.Checked = true;
                        this.DDL_SQLTemplate.SelectedValue= nd.DeliveryParas; // dt.Rows[0]["DeliveryParas"].ToString();
                        break;
                    case DeliveryWay.ByBindEmp: //按绑定的人员.
                        this.RB_ByBindEmp.Checked = true;
                        break;
                    case DeliveryWay.BySelected: //上一步选择.
                        this.RB_BySelected.Checked = true;
                        break;
                    case DeliveryWay.ByPreviousNodeFormEmpsField:
                        this.RB_ByPreviousNodeFormEmpsField.Checked = true;
                        this.DDL_ByPreviousNodeFormEmpsField.SelectedValue = nd.DeliveryParas;
                        break;
                    case DeliveryWay.ByPreviousNodeEmp:
                        this.RB_ByPreviousNodeEmp.Checked = true;
                        break;
                    case DeliveryWay.ByStarter:
                        this.RB_ByStarter.Checked = true;
                        break;
                    case DeliveryWay.BySpecNodeEmp:
                        this.RB_BySpecNodeEmp.Checked = true;

                        string paras = nd.DeliveryParas;
                        string[] strList = paras.Split(',');
                        foreach (string str in strList)
                        {
                            foreach (ListItem item in this.CBL_BySpecNodeEmp.Items)
                            {
                                if (item.Value == str)
                                {
                                    item.Selected = true;
                                }
                            }
                        }
                        break;
                    case DeliveryWay.ByDeptAndStation:
                        this.RB_ByDeptAndStation.Checked = true;
                        break;
                    case DeliveryWay.ByStationAndEmpDept:
                        this.RB_ByStationAndEmpDept.Checked = true;
                        break;
                    case DeliveryWay.BySpecNodeEmpStation:
                        this.RB_BySpecNodeEmpStation.Checked = true;
                        string strRB = nd.DeliveryParas;
                        string[] strDLP = strRB.Split(',');
                        foreach (string str in strDLP)
                        {
                            foreach (ListItem item in this.CBL_BySpecNodeEmpStation.Items)
                            {
                                if (item.Value == str)
                                {
                                    item.Selected = true;
                                }
                            }
                        }
                        break;
                    case DeliveryWay.BySQLAsSubThreadEmpsAndData:
                        this.RB_BySQLAsSubThreadEmpsAndData.Checked = true;
                        this.TB_BySQLAsSubThreadEmpsAndData.Text = nd.DeliveryParas; ;
                        break;
                    case DeliveryWay.ByDtlAsSubThreadEmps:
                        this.RB_ByDtlAsSubThreadEmps.Checked = true;
                        this.TB_ByDtlAsSubThreadEmps.Text = nd.DeliveryParas; ;
                        break;
                    case DeliveryWay.ByStationOnly:
                        this.RB_ByStationOnly.Checked = true;
                        break;
                    case DeliveryWay.ByFEE:
                        this.RB_ByFEE.Checked = true;
                        break;
                    case DeliveryWay.BySetDeptAsSubthread:
                        this.RB_BySetDeptAsSubthread.Checked = true;
                        break;
                    case DeliveryWay.ByCCFlowBPM:
                        this.RB_ByCCFlowBPM.Checked = true;
                        break;
                    case DeliveryWay.ByFromEmpToEmp: //从人员，到人员.
                        this.RB_ByFromEmpToEmp.Checked = true;
                        this.TB_ByFromEmpToEmp.Text = nd.DeliveryParas; // dt.Rows[0]["DeliveryParas"].ToString();
                        break;
                }
            }
        }
        protected void Btn_SaveAndClose_Click(object sender, EventArgs e)
        {
            this.Btn_Save_Click(null,null);
            this.WinClose();
        }
        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            //获取选择的 09.与指定节点处理人相同.
            string strzdjd = "";
            foreach (ListItem li in this.CBL_BySpecNodeEmp.Items)
            {
                if (li.Selected) strzdjd += li.Value + ",";
            }
            strzdjd = strzdjd.TrimEnd(',');

            //12.按指定节点的人员岗位计算
            string strzdjdgw = "";
            foreach (ListItem li in this.CBL_BySpecNodeEmpStation.Items)
            {
                if (li.Selected) strzdjdgw += li.Value + ",";
            }

            strzdjdgw = strzdjdgw.TrimEnd(',');


            Node nd = new Node(this.NodeID);
            string sql = "";
            if (this.RB_ByStation.Checked)
            {
                //按岗位(以部门为纬度)
                nd.HisDeliveryWay = DeliveryWay.ByStation;
                nd.DirectUpdate();
                //按照岗位.
                NodeStations nss = new NodeStations();
                nss.Retrieve(NodeStationAttr.FK_Node, this.NodeID);
                if (nss.Count == 0)
                    PageMessage = "您选择的是按岗位来计算接受人，没有设置岗位无法计算该节点的接受人。";
            }

            if (this.RB_ByDept.Checked)
            {
                // 按部门
                nd.HisDeliveryWay = DeliveryWay.ByDept;
                nd.DirectUpdate();
                //按照岗位.
                NodeDepts nss = new NodeDepts();
                nss.Retrieve(NodeStationAttr.FK_Node, this.NodeID);
                if (nss.Count == 0)
                    PageMessage = "您选择的是按部门来计算接受人，请设置部门没有部门无法计算该节点的接受人。";
            }

            if (this.RB_BySQL.Checked)
            {
                //按SQL
                nd.HisDeliveryWay = DeliveryWay.BySQL;
                nd.DeliveryParas = this.TB_BySQL.Text;
                nd.DirectUpdate();
                sql = this.TB_BySQL.Text;
                if (sql.Length <= 5)
                    PageMessage = "请设置完整的SQL";

                //检查SQL是否符合要求.
                try
                {
                    //替换.避免出错
                    sql = sql.Replace("@WorkID", "0");//工作ID
                    sql = sql.Replace("@FID", "0");//
                    sql = sql.Replace("@OID", "0");
                    sql = sql.Replace("@FK_Node", "0");
                    sql = sql.Replace("@FK_Flow", "0");
                    sql = sql.Replace("@PWorkID", "0");
                    sql = sql.Replace("@PFlowNo", "0");
                    sql = sql.Replace("@PNodeID", "0");

                    sql = BP.WF.Glo.DealExp(sql, null, null);
                    DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    if (dt.Columns.Contains("No") == false || dt.Columns.Contains("Name") == false)
                        PageMessage = "查询结果集合里不包含No,Name两个列, 分别标识操作员编号，操作员名称。";
                }
                catch (Exception ex)
                {
                    PageMessage = "设置的SQL不符合要求SQL=" + sql + ",其他信息：" + ex.Message;
                }
            }

            if (this.RB_BySQLTemplate.Checked)
            {
                //按 RB_BySQLTemplate 
                nd.HisDeliveryWay = DeliveryWay.BySQLTemplate;
                nd.DeliveryParas = this.DDL_SQLTemplate.SelectedValue;
                nd.DirectUpdate();
            }

            if (this.RB_ByBindEmp.Checked)
            {
                //按本节点绑定的人员
                nd.HisDeliveryWay = DeliveryWay.ByBindEmp;
                nd.DirectUpdate();

                //按照岗位.
                NodeEmps nss = new NodeEmps();
                nss.Retrieve(NodeEmpAttr.FK_Node, this.NodeID);
                if (nss.Count == 0)
                    PageMessage = "您选择的是按人员来计算接受人，请设置人员没有人员无法计算该节点的接受人。";

            }
            if (this.RB_BySelected.Checked)
            {
                //由上一步发送人选择
                nd.HisDeliveryWay = DeliveryWay.BySelected;
            }

            if (this.RB_ByPreviousNodeFormEmpsField.Checked)
            {
                //按表单选择人员
                nd.HisDeliveryWay = DeliveryWay.ByPreviousNodeFormEmpsField;
                nd.DeliveryParas = this.DDL_ByPreviousNodeFormEmpsField.SelectedValue;

            }
            if (this.RB_ByPreviousNodeEmp.Checked)
            {
                //与上一节点的人员相同
                nd.HisDeliveryWay = DeliveryWay.ByPreviousNodeEmp;
            }
            if (this.RB_ByStarter.Checked)
            {
                //与开始节点的人员相同
                nd.HisDeliveryWay = DeliveryWay.ByStarter;
            }
            if (this.RB_BySpecNodeEmp.Checked)
            {
                //与指定节点的人员相同
                nd.HisDeliveryWay = DeliveryWay.BySpecNodeEmp;
                nd.DeliveryParas = strzdjd;
            }
            if (this.RB_ByDeptAndStation.Checked)
            {
                //按岗位与部门交集计算
                nd.HisDeliveryWay = DeliveryWay.ByDeptAndStation;
            }
            if (this.RB_ByStationAndEmpDept.Checked)
            {
                //按岗位计算(以部门集合为纬度)
                nd.HisDeliveryWay = DeliveryWay.ByStationAndEmpDept;
            }
            if (this.RB_BySpecNodeEmpStation.Checked)
            {
                //按指定节点的人员或者指定字段作为人员的岗位计算
                nd.HisDeliveryWay = DeliveryWay.BySpecNodeEmpStation;
                nd.DeliveryParas = strzdjdgw;
            }
            if (this.RB_BySQLAsSubThreadEmpsAndData.Checked)
            {
                sql = this.TB_BySQLAsSubThreadEmpsAndData.Text;
                if (sql.Length <= 5)
                    PageMessage = "请设置完整的SQL";
                //按SQL确定子线程接受人与数据源.
                nd.HisDeliveryWay = DeliveryWay.BySQLAsSubThreadEmpsAndData;
                nd.DeliveryParas = this.TB_BySQLAsSubThreadEmpsAndData.Text;

            }
            if (this.RB_ByDtlAsSubThreadEmps.Checked)
            {
                sql = this.TB_ByDtlAsSubThreadEmps.Text;
                if (sql.Length <= 5)
                    PageMessage = "请设置完整的SQL";
                //按明细表确定子线程接受人.
                nd.HisDeliveryWay = DeliveryWay.ByDtlAsSubThreadEmps;
                nd.DeliveryParas = this.TB_ByDtlAsSubThreadEmps.Text;

            }
            if (this.RB_ByStationOnly.Checked)
            {
                //仅按岗位计算.
                nd.HisDeliveryWay = DeliveryWay.ByStationOnly;
            }
            if (this.RB_ByFEE.Checked)
            {
                // FEE计算.
                nd.HisDeliveryWay = DeliveryWay.ByFEE;
            }
            if (this.RB_BySetDeptAsSubthread.Checked)
            {
                //按照按绑定部门计算，该部门一人处理标识该工作结束(子线程)
                nd.HisDeliveryWay = DeliveryWay.BySetDeptAsSubthread;
            }

            if (this.RB_ByCCFlowBPM.Checked)
            {
                //按照ccflow的BPM模式处理.
                nd.HisDeliveryWay = DeliveryWay.ByCCFlowBPM;
            }

            //从人员，到人员的路由.
            if (this.RB_ByFromEmpToEmp.Checked)
            {
                //按 RB_BySQLTemplate 
                nd.HisDeliveryWay = DeliveryWay.ByFromEmpToEmp;
                nd.DeliveryParas = this.TB_ByFromEmpToEmp.Text; //.SelectedValue;
                nd.DirectUpdate();
            }


            //是否可以分配工作？
            nd.IsTask = this.CB_IsSSS.Checked;

            //是否启用自动记忆功能
            nd.IsRememberMe = this.CB_IsRememme.Checked;

            //本节点接收人不允许包含上一步发送人
            nd.IsExpSender = this.CB_IsExpSender.Checked;
            //发送后转向
             
            //清除发起列表的缓存.
            if (nd.IsStartNode == true)
            {
                DBAccess.RunSQL("UPDATE WF_Emp SET StartFlows='' ");
                if (SystemConfig.CustomerNo == "TianYe")
                    DBAccess.RunSQL("UPDATE WF_FlowSort SET OrgNo='0' WHERE WF_FlowSort.OrgNo='101'");
            }

            nd.DirectUpdate();
        }
        /// <summary>
        /// 06.按上一节点表单指定的字段值作为本步骤的接受人 参数: 请在上一节点表单创建SysSendEmps文本框.
        /// </summary>
        public void bindDDL_5()
        {
            string str = this.NodeID.ToString().Substring(0, this.NodeID.ToString().Length - 2);
            BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, "ND" + str + "Rpt", MapAttrAttr.KeyOfEn);

            foreach (BP.Sys.MapAttr item in attrs)
            {
                this.DDL_ByPreviousNodeFormEmpsField.Items.Add(new ListItem(item.KeyOfEn + " " + item.Name, item.KeyOfEn));
            }
        }
        /// <summary>
        /// 09.与指定节点处理人相同 取值
        /// 12.按指定节点的人员岗位计算
        /// </summary>
        public void bindCBL()
        {
            BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            this.CBL_BySpecNodeEmp.Items.Clear();
            this.CBL_BySpecNodeEmpStation.Items.Clear();
            BP.WF.Nodes nds = new BP.WF.Nodes(nd.HisFlow.No);
            foreach (BP.WF.Node item in nds)
            {
                this.CBL_BySpecNodeEmp.Items.Add(new ListItem(item.NodeID + " " + item.Name, item.NodeID.ToString()));
                this.CBL_BySpecNodeEmpStation.Items.Add(new ListItem(item.NodeID + " " + item.Name, item.NodeID.ToString()));
            }
        }
    }
}