using System;
using System.Data;
using System.Linq;
using System.Collections;
using BP.Sys;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Port;
using BP.WF.Data;
using BP.WF.Template;
using System.IO;
using BP.Sys.FrmUI;
using System.Text.RegularExpressions;
using BP.Difference;
using BP.WF.Template.SFlow;
using BP.WF.Template.Frm;
using System.ServiceModel.Security;
using System.Runtime.CompilerServices;

namespace BP.WF
{
    /// <summary>
    /// 全局(方法处理)
    /// </summary>
    public class Glo
    {

        #region 新建节点-流程-默认值.

        #endregion 默认值.
        /// <summary>
        /// 单据编号对应字段SQL
        /// </summary>
        public static string SQLOfBillNo
        {
            get
            {
                string sql = "";
                switch (BP.Difference.SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                    case DBType.MySQL:
                        sql = "SELECT '' AS No, '-请选择-' as Name ";
                        break;
                    case DBType.Oracle:
                    case DBType.KingBaseR3:
                    case DBType.KingBaseR6:
                        sql = "SELECT '' AS No, '-请选择-' as Name FROM DUAL ";
                        break;
                    case DBType.PostgreSQL:
                    case DBType.UX:
                    case DBType.HGDB:
                    default:
                        sql = "SELECT '' AS No, '-请选择-' as Name FROM Port_Emp WHERE 1=2 ";
                        break;
                }
                sql += " UNION ";
                sql += " SELECT KeyOfEn AS No,Name FROM Sys_MapAttr WHERE UIContralType=0 AND UIVisible=1 AND UIIsEnable=1 AND FK_MapData='@FK_Frm'";
                return sql;
            }
        }
        /// <summary>
        /// 签批组件SQL
        /// </summary>
        public static string SQLOfCheckField
        {
            get
            {
                string sql = "";
                switch (BP.Difference.SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                    case DBType.MySQL:
                        sql = "SELECT '' AS No, '-请选择-' as Name ";
                        break;
                    case DBType.Oracle:
                    case DBType.KingBaseR3:
                    case DBType.KingBaseR6:
                        sql = "SELECT '' AS No, '-请选择-' as Name FROM DUAL ";
                        break;
                    case DBType.PostgreSQL:
                    case DBType.UX:
                    case DBType.HGDB:
                    default:
                        sql = "SELECT '' AS No, '-请选择-' as Name FROM Port_Emp WHERE 1=2 ";
                        break;
                }
                sql += " UNION ";
                sql += " SELECT KeyOfEn AS No,Name From Sys_MapAttr WHERE UIContralType=14 AND FK_MapData='@FK_Frm'";
                return sql;
            }
        }

        #region 获取[新建-节点-流程]默认值.
        /// <summary>
        /// 新建节点的审核意见默认值.
        /// </summary>
        public static string DefVal_WF_Node_FWCDefInfo
        {
            get
            {
                return BP.Difference.SystemConfig.GetValByKey("DefVal_WF_Node_FWCDefInfo", "同意");
            }
        }
        #endregion 获取[新建流程]默认值.


        #region 多语言处理.
        private static Hashtable _Multilingual_Cache = null;
        public static DataTable getMultilingual_DT(string className)
        {
            if (_Multilingual_Cache == null)
                _Multilingual_Cache = new Hashtable();

            if (_Multilingual_Cache.ContainsKey(className) == false)
            {
                DataSet ds = DataType.CXmlFileToDataSet(BP.Difference.SystemConfig.PathOfData + "lang/xml/" + className + ".xml");
                DataTable dt = ds.Tables[0];

                _Multilingual_Cache.Add(className, dt);
            }

            return _Multilingual_Cache[className] as DataTable;
        }
        /// <summary>
        /// 转换语言.
        /// </summary>
        public static string multilingual(string defaultMsg, string className, string key, string p0 = null, string p1 = null, string p2 = null, string p3 = null)
        {
            int num = 4;
            string[] paras = new string[num];
            if (p0 != null)
                paras[0] = p0;

            if (p1 != null)
                paras[1] = p1;

            if (p2 != null)
                paras[2] = p2;

            if (p3 != null)
                paras[3] = p3;

            return multilingual(defaultMsg, className, key, paras);
        }
        /// <summary>
        /// 获取多语言
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="key"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public static string multilingual(string defaultMsg, string className, string key, string[] paramList)
        {
            if (BP.Web.WebUser.SysLang.Equals("zh-cn") || BP.Web.WebUser.SysLang.Equals("CH"))
                return String.Format(defaultMsg, paramList);

            DataTable dt = getMultilingual_DT(className);

            string val = "";
            foreach (DataRow dr in dt.Rows)
            {
                if ((string)dr.ItemArray[0] == key)
                {
                    switch (BP.Web.WebUser.SysLang)
                    {
                        case "zh-cn":
                            val = (string)dr.ItemArray[1];
                            break;
                        case "zh-tw":
                            val = (string)dr.ItemArray[2];
                            break;
                        case "zh-hk":
                            val = (string)dr.ItemArray[3];
                            break;
                        case "en-us":
                            val = (string)dr.ItemArray[4];
                            break;
                        case "ja-jp":
                            val = (string)dr.ItemArray[5];
                            break;
                        case "ko-kr":
                            val = (string)dr.ItemArray[6];
                            break;
                        default:
                            val = (string)dr.ItemArray[1];
                            break;
                    }
                    break;
                }
            }
            return String.Format(val, paramList);
        }


        //public static void Multilingual_Demo()
        //{
        //    //普通的多语言处理.
        //    string msg = "您确定要删除吗？";
        //    msg = BP.WF.Glo.Multilingual_Public(msg, "confirm");


        //    //带有参数的语言处理..
        //    msg = "您确定要删除吗？删除{0}后，就不能恢复。";
        //    msg = BP.WF.Glo.Multilingual_Public(msg, "confirmDel", "zhangsan");

        //    //   BP.WF.Glo.Multilingual_Public("confirm",
        //}


        #endregion 多语言处理.

        #region 公共属性.
        /// <summary>
        /// 打印文件
        /// </summary>
        public static string PrintBackgroundWord
        {
            get
            {
                string s = BP.Difference.SystemConfig.AppSettings["PrintBackgroundWord"];
                if (string.IsNullOrEmpty(s))
                    s = "驰骋工作流引擎@开源驰骋 - ccflow@openc";
                return s;
            }
        }
        /// <summary>
        /// 运行平台.
        /// </summary>
        public static Platform Platform
        {
            get
            {
                return Platform.CCFlow;
            }
        }

        /// <summary>
        /// 短消息写入类型
        /// </summary>
        public static ShortMessageWriteTo ShortMessageWriteTo
        {
            get
            {
                return (ShortMessageWriteTo)SystemConfig.GetValByKeyInt("ShortMessageWriteTo", 0);
            }
        }
        /// <summary>
        /// 当前选择的流程.
        /// </summary>
        public static string CurrFlow
        {
            get
            {
                return HttpContextHelper.SessionGet("CurrFlow") as string;
            }
            set
            {
                HttpContextHelper.SessionSet("CurrFlow", value);
            }
        }
        /// <summary>
        /// 用户编号.
        /// </summary>
        public static string UserNo = null;
        /// <summary>
        /// 运行平台(用于处理不同的平台，调用不同的URL)
        /// </summary>
        public static Plant Plant = WF.Plant.CCFlow;
        #endregion 公共属性.

        /// <summary>
        /// CCBPMRunModel
        /// </summary>
        public static CCBPMRunModel CCBPMRunModel
        {
            get
            {
                return BP.Difference.SystemConfig.CCBPMRunModel;
            }
        }

        #region 执行安装/升级.
        /// <summary>
        /// 当前版本号-为了升级使用.
        /// </summary>
        public static int Ver = 20230816;
        /// <summary>
        /// 执行升级
        /// </summary>
        /// <returns>执行升级的结果</returns>
        public static string UpdataCCFlowVer()
        {
            if (Glo.CCBPMRunModel == CCBPMRunModel.SAAS)
                return "info@SAAS模式需要手工升级.";

            #region 检查是否需要升级，并更新升级的业务逻辑.
            string updataNote = "";
            /*
             * 升级版本记录:
             * 20150330: 优化发起列表的效率, by:zhoupeng.
             * 2, 升级表单树,支持动态表单树.
             * 1, 执行一次Sender发送人的升级，原来由GenerWorkerList 转入WF_GenerWorkFlow.
             * 0, 静默升级启用日期.2014-12
             */
            if (DBAccess.IsExitsObject("Sys_Serial") == false)
                return "";

            //升级SQL
            UpdataCCFlowVerSQLScript();

            //判断数据库的版本.
            string sql = "SELECT IntVal FROM Sys_Serial WHERE CfgKey='Ver'";
            int currDBVer = DBAccess.RunSQLReturnValInt(sql, 0);
            if (currDBVer != null && currDBVer != 0 && currDBVer >= Ver)
                return null; //不需要升级.
            #endregion 检查是否需要升级，并更新升级的业务逻辑.

            //执行sql.升级节点高度.
            if (DBAccess.IsExitsTableCol("WF_Node", "UIWidth") == true)
                DBAccess.RunSQL("UPDATE WF_Node SET UIWidth=120,UIHeight=60 WHERE UIWidth=0 OR UIWidth Is Null ");

            #region 2023.07.02 升级字典表,查询.
            SFSearch search = new SFSearch();
            search.CheckPhysicsTable();
            Sys.SFTable table = new Sys.SFTable();
            table.CheckPhysicsTable();
            SFProcedure enProduce = new SFProcedure();
            enProduce.CheckPhysicsTable();
            #endregion 2023.07.02 

            #region 升级SFTable中SrcType为DBSrcType 
            if (DBAccess.IsExitsTableCol("Sys_SFTable", "SrcType") == true)
            {
                if (DBAccess.IsExitsTableCol("Sys_SFTable", "DBSrcType") == false)
                {
                    switch (SystemConfig.AppCenterDBType)
                    {
                        case DBType.MSSQL:
                            DBAccess.RunSQL("ALTER TABLE Sys_SFTable ADD DBSrcType NVARCHAR(20) DEFAULT 'BPClass' NULL");
                            break;
                        case DBType.Oracle:
                        case DBType.Informix:
                        case DBType.PostgreSQL:
                        case DBType.HGDB:
                        case DBType.UX:
                        case DBType.KingBaseR3:
                        case DBType.KingBaseR6:
                            DBAccess.RunSQL("ALTER TABLE Sys_SFTable ADD DBSrcType VARCHAR(20) DEFAULT 'BPClass' NULL");
                            break;
                        case DBType.MySQL:
                            DBAccess.RunSQL("ALTER TABLE Sys_SFTable ADD DBSrcType NVARCHAR(20) DEFAULT 'BPClass' NULL");
                            break;
                        default:
                            break;
                    }

                }
                DBAccess.RunSQL("UPDATE Sys_SFTable SET DBSrcType=(CASE SrcType WHEN  0 THEN 'BPClass' WHEN 1 THEN 'CreateTable' WHEN 1 THEN 'CreateTable' " +
                    "WHEN 2 THEN 'TableOrView' WHEN 3 THEN 'SQL' WHEN 4 THEN 'WebServices' WHEN 5 THEN 'Handler' WHEN 6 THEN 'JQuery' " +
                    "WHEN 7 THEN 'SysDict' ELSE 'WebApi' END)");
                DBAccess.DropTableColumn("Sys_SFTable", "SrcType");

            }
            #endregion 升级SFTable中SrcType为DBSrcType

            #region 升级流程模式的存储方式
            if (DBAccess.IsExitsTableCol("WF_Flow", "FlowDevModel") == false)
            {
                switch (SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                        DBAccess.RunSQL("ALTER TABLE WF_Flow ADD FlowDevModel INT NULL");
                        break;
                    case DBType.Oracle:
                    case DBType.Informix:
                    case DBType.PostgreSQL:
                    case DBType.HGDB:
                    case DBType.UX:
                    case DBType.KingBaseR3:
                    case DBType.KingBaseR6:
                        DBAccess.RunSQL("ALTER TABLE WF_Flow ADD FlowDevModel INTEGER NULL");
                        break;
                    case DBType.MySQL:
                        DBAccess.RunSQL("ALTER TABLE WF_Flow ADD FlowDevModel INT NULL");
                        break;
                    default:
                        break;
                }
            }
            Flows flows = new Flows();
            QueryObject qo = new QueryObject(flows);
            qo.AddWhere("AtPara", "Like", "%FlowDevModel=%");
            qo.DoQuery();
            foreach (Flow flow in flows)
            {
                int flowDevModel = flow.GetParaInt("FlowDevModel");
                flow.FlowDevModel = (FlowDevModel)flowDevModel;
                string atPara = flow.GetValStringByKey("AtPara");
                atPara = atPara.Replace("@FlowDevModel=" + flowDevModel, "");
                flow.SetValByKey("AtPara", atPara);
                flow.DirectUpdate();
            }
            #endregion 升级流程模式的存储方式

            #region 升级文本框字段类型.  TextModel=0普通文本，1密码，2=TextArea,3=富文本.
            //说明没有升级. TextModel=0
            if (DBAccess.IsExitsTableCol("Sys_MapAttr", "IsRichText") == true)
            {
                MapAttr ma = new MapAttr();
                ma.CheckPhysicsTable();

                sql = "UPDATE Sys_MapAttr SET TextModel=3 WHERE IsRichText=1 OR AtPara LIKE '%IsRichText=1%'";
                DBAccess.RunSQL(sql);

                sql = "UPDATE Sys_MapAttr SET TextModel=2 WHERE UIHeight >=40 OR IsSupperText=1";
                DBAccess.RunSQL(sql);

                sql = "UPDATE Sys_MapAttr SET TextModel=1 WHERE IsSecret=1";
                DBAccess.RunSQL(sql);

                DBAccess.DropTableColumn("Sys_MapAttr", "IsRichText");
                DBAccess.DropTableColumn("Sys_MapAttr", "IsSecret");
            }
            #endregion 升级文本框字段类型

            #region 统一升级主键. 给多对多的实体增加主键.
            if (DBAccess.IsExitsTableCol("WF_NodeStation", "MyPK") == false)
            {
                //1.首先要删除主键.
                DBAccess.DropTablePK("WF_NodeStation");
                if (BP.Difference.SystemConfig.AppCenterDBType.ToString() == "MySQL")
                {
                    DBAccess.RunSQL("ALTER TABLE WF_NodeStation ADD COLUMN MyPK VARCHAR(100)");
                    DBAccess.RunSQL("UPDATE WF_NodeStation SET MyPK= CONCAT(FK_Node,'_',FK_Station)");
                }
                else
                    if (BP.Difference.SystemConfig.AppCenterDBType.ToString() == "MSSQL")
                {
                    DBAccess.RunSQL("ALTER TABLE WF_NodeStation ADD  MyPK VARCHAR(100)");
                    DBAccess.RunSQL("UPDATE WF_NodeStation SET MyPK= CONVERT(varchar,FK_Node)+'_'+FK_Station");
                }

                //2. 自动创建.
                NodeStation ns = new NodeStation();
                ns.CheckPhysicsTable();
            }

            if (DBAccess.IsExitsObject("WF_NodeTeam") == false)
            {
                NodeTeam nt = new NodeTeam();
                nt.CheckPhysicsTable();
            }

            if (DBAccess.IsExitsTableCol("WF_NodeTeam", "MyPK") == false)
            {
                DBAccess.DropTablePK("WF_NodeTeam");
                if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
                {
                    DBAccess.RunSQL("ALTER TABLE WF_NodeTeam ADD COLUMN MyPK VARCHAR(100)");
                    DBAccess.RunSQL("UPDATE WF_NodeTeam SET MyPK= CONCAT(FK_Node,'_',FK_Team)");
                }
                else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MSSQL)
                {
                    DBAccess.RunSQL("ALTER TABLE WF_NodeTeam ADD  MyPK VARCHAR(100)");
                    DBAccess.RunSQL("UPDATE WF_NodeTeam SET MyPK= CONVERT(varchar,FK_Node)+'_'+FK_Team");
                }
                //3. 执行更新.
            }

            if (DBAccess.IsExitsTableCol("WF_NodeEmp", "MyPK") == false)
            {

                DBAccess.DropTablePK("WF_NodeEmp");
                if (BP.Difference.SystemConfig.AppCenterDBType.ToString() == "MySQL")
                {
                    DBAccess.RunSQL("ALTER TABLE WF_NodeEmp ADD COLUMN MyPK VARCHAR(100)");
                    DBAccess.RunSQL("UPDATE WF_NodeEmp SET MyPK= CONCAT(FK_Node,'_',FK_Emp)");
                }
                else
           if (BP.Difference.SystemConfig.AppCenterDBType.ToString() == "MSSQL")
                {

                    DBAccess.RunSQL("ALTER TABLE WF_NodeEmp ADD  MyPK VARCHAR(100)");
                    DBAccess.RunSQL("UPDATE WF_NodeEmp SET MyPK= CONVERT(varchar,FK_Node)+'_'+FK_Emp");
                }
                NodeEmp ne1 = new NodeEmp();
                ne1.CheckPhysicsTable();
                //3. 执行更新.

            }
            #endregion 统一升级主键.

            #region 系统更新.
            //升级支持ts.
            // UpdataTSModel();
            //升级日志.
            UserLogLevel ul = new UserLogLevel();
            ul.CheckPhysicsTable();
            UserLogType ut = new UserLogType();
            ut.CheckPhysicsTable();

            //添加IsEnable
            //BP.WF.Template.FlowTab fb = new BP.WF.Template.FlowTab();
            //fb.CheckPhysicsTable();

            if (DBAccess.IsExitsTableCol("Sys_GroupField", "EnName") == true)
            {
                GroupField groupField = new GroupField();
                groupField.CheckPhysicsTable();
                DBAccess.RunSQL("UPDATE Sys_GroupField SET FrmID=enName WHERE FrmID IS null");
            }

            //升级.
            Auth ath = new Auth();
            ath.CheckPhysicsTable();

            //检查BPM.现在暂时不使用原菜单结构
            // if (!SystemConfig.OrganizationIsView)
            //    CheckGPM();

            MapData mapData = new MapData();
            mapData.CheckPhysicsTable();

            Direction dir = new Direction();
            dir.CheckPhysicsTable();
            #endregion 系统更新.

            #region 升级优化集团版的应用. 2020.04.03

            //--2020.05.28 升级方向条件;
            BP.WF.Template.Cond cond = new Cond();
            cond.CheckPhysicsTable();
            if (DBAccess.IsExitsTableCol("WF_Cond", "PRI") == true)
            {
                DBAccess.RunSQL("UPDATE WF_Cond SET Idx=PRI ");
                DBAccess.DropTableColumn("WF_Cond", "PRI");
            }

            //修改节点类型,合并属性.
            /*if (DBAccess.IsExitsTableCol("WF_Node", "SubThreadType") == true)
            {
                DBAccess.RunSQLReturnTable("UPDATE WF_Node SET RunModel=5 WHERE SubThreadType=1 ");
                DBAccess.DropTableColumn("WF_Node", "SubThreadType");
            }*/

            //--2020.05.25 修改节点自定义按钮功能;
            BP.WF.Template.NodeToolbar bar = new NodeToolbar();
            bar.CheckPhysicsTable();
            if (DBAccess.IsExitsTableCol("WF_NodeToolbar", "ShowWhere") == true)
            {
                DBAccess.RunSQL("UPDATE WF_NodeToolbar SET IsMyFlow = 1 Where ShowWhere = 1");
                DBAccess.RunSQL("UPDATE WF_NodeToolbar SET IsMyCC = 1 Where ShowWhere = 2");

                DBAccess.DropTableColumn("WF_NodeToolbar", "ShowWhere");
            }
            switch (BP.Difference.SystemConfig.AppCenterDBType)
            {
                case DBType.Oracle:
                    DBAccess.RunSQL("UPDATE Sys_MapAttr set tag=''");
                    break;
                default:
                    break;
            }
            Direction direction = new Direction();
            direction.CheckPhysicsTable();

            MapAttr myattr = new MapAttr();
            myattr.CheckPhysicsTable();

            MapDtlExt mde = new MapDtlExt();
            mde.CheckPhysicsTable();

            NodeExt ne = new NodeExt();
            ne.CheckPhysicsTable();

            FlowExt fe = new FlowExt();
            fe.CheckPhysicsTable();

            //检查frmTrack.
            BP.CCBill.Track tk = new BP.CCBill.Track();
            tk.CheckPhysicsTable();

            BP.Port.DeptEmpStation des = new BP.Port.DeptEmpStation();
            des.CheckPhysicsTable();

            BP.Port.DeptEmp de = new BP.Port.DeptEmp();
            de.CheckPhysicsTable();

            BP.Port.Emp emp1 = new BP.Port.Emp();
            emp1.CheckPhysicsTable();

            BP.WF.Port.Admin2Group.Org org = new BP.WF.Port.Admin2Group.Org();
            org.CheckPhysicsTable();

            BP.WF.Template.FlowSort fs = new BP.WF.Template.FlowSort();
            fs.CheckPhysicsTable();

            FlowOrg fo = new FlowOrg();
            fo.CheckPhysicsTable();

            BP.Sys.SysEnumMain sem = new SysEnumMain();
            sem.CheckPhysicsTable();

            BP.Sys.SysEnum myse = new SysEnum();
            myse.CheckPhysicsTable();

            //检查表.
            BP.Sys.GloVar gv = new GloVar();
            gv.CheckPhysicsTable();

            //检查表.
            BP.Sys.EnCfg cfg = new BP.Sys.EnCfg();
            cfg.CheckPhysicsTable();

            //检查表.
            SysFormTree frmTree = new SysFormTree();
            frmTree.CheckPhysicsTable();

            BP.Sys.SFTable sf = new BP.Sys.SFTable();
            sf.CheckPhysicsTable();

            FrmSubFlow sb = new FrmSubFlow();
            sb.CheckPhysicsTable();

            BP.WF.Template.PushMsg pm = new PushMsg();
            pm.CheckPhysicsTable();

            //修复数据表.
            BP.Sys.GroupField gf = new GroupField();
            gf.CheckPhysicsTable();

            #endregion 升级优化集团版的应用

            #region 升级子流程.
            //检查子流程表.
            if (DBAccess.IsExitsObject("WF_NodeSubFlow") == true)
            {
                if (DBAccess.IsExitsTableCol("WF_NodeSubFlow", "OID") == true)
                {
                    DBAccess.RunSQL("DROP TABLE WF_NodeSubFlow");
                    SubFlowHand sub = new SubFlowHand();
                    sub.CheckPhysicsTable();
                }
            }
            #endregion 升级子流程.

            #region 升级方向条件. 2020.06.02
            if (DBAccess.IsExitsTableCol("WF_Cond", "CondOrAnd") == true)
            {
                DataTable dt = DBAccess.RunSQLReturnTable("SELECT DISTINCT FK_Node, toNodeID, CondOrAnd, CondType  FROM wf_cond WHERE DataFrom!=100 ");
                foreach (DataRow dr in dt.Rows)
                {
                    int nodeID = int.Parse(dr["FK_Node"].ToString());
                    int toNodeID = int.Parse(dr["toNodeID"].ToString());

                    Conds conds = new Conds();
                    conds.Retrieve(CondAttr.FK_Node, nodeID,
                        CondAttr.ToNodeID, toNodeID, CondAttr.Idx);

                    //判断是否需要修复？
                    if (conds.Count == 1 || conds.Count == 0)
                        continue;

                    //判断是否有？
                    bool isHave = false;
                    foreach (Cond myCond in conds)
                    {
                        if (myCond.HisDataFrom == ConnDataFrom.CondOperator)
                            isHave = true;
                    }
                    if (isHave == true)
                        continue;

                    //获得类型.
                    int OrAndType = DBAccess.RunSQLReturnValInt("SELECT  CondOrAnd  FROM wf_cond WHERE FK_Node=" + nodeID, -1);
                    if (OrAndType == -1)
                        continue;

                    int idx = 0;
                    int idxSave = 0;
                    int count = conds.Count;
                    foreach (Cond item in conds)
                    {
                        idx++;

                        idxSave++;
                        item.Idx = idxSave;
                        item.Update();

                        if (count == idx)
                            continue;

                        Cond operCond = new Cond();
                        operCond.Copy(item);
                        operCond.setMyPK(DBAccess.GenerGUID());
                        operCond.HisDataFrom = ConnDataFrom.CondOperator;

                        if (OrAndType == 0)
                        {
                            operCond.OperatorValue = " OR ";
                            operCond.Note = " OR ";
                            operCond.OperatorValue = " OR ";
                            operCond.Note = " OR ";
                        }
                        else
                        {
                            operCond.OperatorValue = " AND ";
                            operCond.Note = " AND ";
                            operCond.OperatorValue = " AND ";
                            operCond.Note = " AND ";
                        }

                        idxSave++;
                        operCond.Idx = idxSave;
                        operCond.Insert();
                    }
                }

                //升级后删除指定的列.
                DBAccess.DropTableColumn("WF_Cond", "CondOrAnd");
                DBAccess.DropTableColumn("WF_Cond", "NodeID");
            }
            #endregion 升级方向条件.

            #region 重新生成view WF_EmpWorks,  2013-08-06.
            Glo.ReCreateView();
            #endregion


            #region 升级视图. 解决厦门信息港的 - 流程监控与授权.
            if (DBAccess.IsExitsObject("V_MyFlowData") == false)
            {
                BP.WF.Template.PowerModel pm11 = new PowerModel();
                pm11.CheckPhysicsTable();

                sql = "CREATE VIEW V_MyFlowData ";
                sql += " AS ";
                sql += " SELECT A.*, B.EmpNo as MyEmpNo FROM WF_GenerWorkflow A, WF_PowerModel B ";
                sql += " WHERE  A.FK_Flow=B.FlowNo AND B.PowerCtrlType=1 AND A.WFState>= 2";
                sql += "    UNION  ";
                sql += " SELECT A.*, c.No as MyEmpNo FROM WF_GenerWorkflow A, WF_PowerModel B, Port_Emp C, Port_DeptEmpStation D";
                sql += " WHERE  A.FK_Flow=B.FlowNo  AND B.PowerCtrlType=0 AND C.No=D.FK_Emp AND B.StaNo=D.FK_Station AND A.WFState>=2";
                DBAccess.RunSQL(sql);
            }
            #endregion 升级视图.

            //升级从表的 fk_node .
            //获取需要修改的从表
            string dtlNos = DBAccess.RunSQLReturnString("SELECT B.NO  FROM SYS_GROUPFIELD A, SYS_MAPDTL B WHERE A.CTRLTYPE='Dtl' AND A.CTRLID=B.NO AND FK_NODE!=0");
            if (DataType.IsNullOrEmpty(dtlNos) == false)
            {
                dtlNos = dtlNos.Replace(",", "','");
                dtlNos = "('" + dtlNos + "')";
                DBAccess.RunSQL("UPDATE SYS_MAPDTL SET FK_NODE=0 WHERE NO IN " + dtlNos);
            }
            FrmNode nff = new FrmNode();
            nff.CheckPhysicsTable();

            #region 更新节点名称.
            switch (SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                case DBType.PostgreSQL:
                case DBType.HGDB:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    sql = " UPDATE WF_Direction SET ToNodeName = WF_Node.Name FROM WF_Node ";
                    sql += " WHERE WF_Direction.ToNode = WF_Node.NodeID ";
                    break;
                case DBType.Oracle:
                    sql = "UPDATE WF_Direction E SET ToNodeName=(SELECT U.Name FROM WF_Node U WHERE E.ToNode=U.NodeID) WHERE EXISTS (SELECT 1 FROM WF_Node U WHERE E.ToNode=U.NodeID)";
                    break;
                default:
                    sql = "UPDATE WF_Direction A, WF_Node B SET A.ToNodeName=B.Name WHERE A.ToNode=B.NodeID ";
                    break;
            }
            DBAccess.RunSQL(sql);

            //更新groupField.
            switch (BP.Difference.SystemConfig.AppCenterDBType)
            {
                case DBType.MySQL:
                    sql = "UPDATE Sys_MapDtl, Sys_GroupField B SET Sys_MapDtl.GroupField=B.OID WHERE Sys_MapDtl.No=B.CtrlID AND Sys_MapDtl.GroupField=''";
                    break;
                case DBType.Oracle:
                    sql = "UPDATE Sys_MapDtl E SET GroupField=(SELECT U.OID FROM Sys_GroupField U WHERE E.No=U.CtrlID) WHERE EXISTS (SELECT 1 FROM Sys_GroupField U WHERE E.No=U.CtrlID AND E.GroupField='')";
                    DBAccess.RunSQL("UPDATE Sys_MapAttr set tag=''");
                    break;
                case DBType.MSSQL:
                default:
                    sql = "UPDATE Sys_MapDtl SET GroupField=Sys_GroupField.OID FROM Sys_GroupField WHERE Sys_MapDtl.No=Sys_GroupField.CtrlID AND Sys_MapDtl.GroupField=''";
                    break;
            }
            DBAccess.RunSQL(sql);
            #endregion 更新节点名称.

            #region 升级审核组件
            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
                sql = "UPDATE WF_FrmNode F INNER JOIN(SELECT FWCSta,NodeID FROM WF_Node ) N on F.FK_Node = N.NodeID and  F.IsEnableFWC =1 SET F.IsEnableFWC = N.FWCSta;";
            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MSSQL)
                sql = "UPDATE    F SET IsEnableFWC = N. FWCSta  FROM WF_FrmNode F,WF_Node N    WHERE F.FK_Node = N.NodeID AND F.IsEnableFWC =1";
            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR3 || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR6)
                sql = "UPDATE WF_FrmNode F  SET (IsEnableFWC)=(SELECT FWCSta FROM WF_Node N WHERE F.FK_Node = N.NodeID AND F.IsEnableFWC =1)";
            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.HGDB || BP.Difference.SystemConfig.AppCenterDBType == DBType.HGDB || BP.Difference.SystemConfig.AppCenterDBType == DBType.UX)
                sql = "UPDATE WF_FrmNode SET IsEnableFWC=(SELECT FWCSta FROM WF_Node N Where N.NodeID=WF_FrmNode.FK_Node AND WF_FrmNode.IsEnableFWC=1)";

            DBAccess.RunSQL(sql);
            #endregion 升级审核组件

            #region 升级填充数据.
            //pop自动填充
            MapExts exts = new MapExts();
            qo = new QueryObject(exts);
            qo.AddWhere(MapExtAttr.ExtType, " LIKE ", "Pop%");
            qo.DoQuery();
            foreach (MapExt ext in exts)
            {
                string mypk = ext.FrmID + "_" + ext.AttrOfOper;
                MapAttr ma = new MapAttr();
                ma.setMyPK(mypk);
                if (ma.RetrieveFromDBSources() == 0)
                {
                    ext.Delete();
                    continue;
                }

                if (ma.GetParaString("PopModel") == ext.ExtType)
                    continue; //已经修复了，或者配置了.

                ma.SetPara("PopModel", ext.ExtType);
                ma.Update();

                if (DataType.IsNullOrEmpty(ext.Tag4) == true)
                    continue;

                MapExt extP = new MapExt();
                extP.setMyPK(ext.MyPK + "_FullData");
                int i = extP.RetrieveFromDBSources();
                if (i == 1)
                    continue;

                extP.ExtType = "FullData";
                extP.FrmID = ext.FrmID;
                extP.AttrOfOper = ext.AttrOfOper;
                extP.DBType = ext.DBType;
                extP.Doc = ext.Tag4;
                extP.Insert(); //执行插入.
            }


            //文本自动填充
            exts = new MapExts();
            exts.Retrieve(MapExtAttr.ExtType, MapExtXmlList.TBFullCtrl);
            foreach (MapExt ext in exts)
            {
                string mypk = ext.FrmID + "_" + ext.AttrOfOper;
                MapAttr ma = new MapAttr();
                ma.setMyPK(mypk);
                if (ma.RetrieveFromDBSources() == 0)
                {
                    ext.Delete();
                    continue;
                }
                string modal = ma.GetParaString("TBFullCtrl");
                if (DataType.IsNullOrEmpty(modal) == false)
                    continue; //已经修复了，或者配置了.

                if (DataType.IsNullOrEmpty(ext.Tag3) == false)
                    ma.SetPara("TBFullCtrl", "Simple");
                else
                    ma.SetPara("TBFullCtrl", "Table");

                ma.Update();

                if (DataType.IsNullOrEmpty(ext.Tag4) == true)
                    continue;

                MapExt extP = new MapExt();
                extP.setMyPK(ext.MyPK + "_FullData");
                int i = extP.RetrieveFromDBSources();
                if (i == 1)
                    continue;

                extP.ExtType = "FullData";
                extP.FrmID = ext.FrmID;
                extP.AttrOfOper = ext.AttrOfOper;
                extP.DBType = ext.DBType;
                extP.Doc = ext.Tag4;

                //填充从表
                extP.Tag1 = ext.Tag1;
                //填充下拉框
                extP.Tag = ext.Tag;

                extP.Insert(); //执行插入.
            }

            //下拉框填充其他控件
            //文本自动填充
            exts = new MapExts();
            exts.Retrieve(MapExtAttr.ExtType, MapExtXmlList.DDLFullCtrl);
            foreach (MapExt ext in exts)
            {
                string mypk = ext.FrmID + "_" + ext.AttrOfOper;
                MapAttr ma = new MapAttr();
                ma.setMyPK(mypk);
                if (ma.RetrieveFromDBSources() == 0)
                {
                    ext.Delete();
                    continue;
                }
                string modal = ma.GetParaString("IsFullData");
                if (DataType.IsNullOrEmpty(modal) == false)
                    continue; //已经修复了，或者配置了.

                //启用填充其他控件
                ma.SetPara("IsFullData", 1);
                ma.Update();


                MapExt extP = new MapExt();
                extP.setMyPK(ext.MyPK + "_FullData");
                int i = extP.RetrieveFromDBSources();
                if (i == 1)
                    continue;

                extP.ExtType = "FullData";
                extP.FrmID = ext.FrmID;
                extP.AttrOfOper = ext.AttrOfOper;
                extP.DBType = ext.DBType;
                extP.Doc = ext.Doc;


                //填充从表
                extP.Tag1 = ext.Tag1;
                //填充下拉框
                extP.Tag = ext.Tag;

                extP.Insert(); //执行插入.

            }

            //装载填充
            exts = new MapExts();
            exts.Retrieve(MapExtAttr.ExtType, MapExtXmlList.PageLoadFull);
            foreach (MapExt ext in exts)
            {
                mapData.No = ext.FrmID;
                if (mapData.RetrieveFromDBSources() == 0)
                {
                    ext.Delete();
                    continue;
                }
                if (DataType.IsNullOrEmpty(mapData.GetParaString("IsPageLoadFull")) == false)
                    continue;
                mapData.ItIsPageLoadFull = true;
                mapData.Update();

                //修改填充数据的值
                ext.Doc = ext.Tag;

                string tag1 = ext.Tag1;
                if (DataType.IsNullOrEmpty(tag1) == true)
                {
                    ext.Update();
                    continue;
                }
                MapDtls dtls = mapData.MapDtls;
                foreach (MapDtl dtl in dtls)
                {
                    tag1 = tag1.Replace("*" + dtl.No + "=", "$" + dtl.No + ":");
                }
                ext.Tag1 = tag1;
                ext.Update();
            }
            #endregion 升级 填充数据.

            string msg = "";
            try
            {

                #region 升级事件.
                if (DBAccess.IsExitsTableCol("Sys_FrmEvent", "DoType") == true)
                {
                    BP.Sys.FrmEvent frmevent = new FrmEvent();
                    frmevent.CheckPhysicsTable();

                    DBAccess.RunSQL("UPDATE Sys_FrmEvent SET EventDoType=DoType  ");
                    DBAccess.RunSQL("ALTER TABLE Sys_FrmEvent  DROP COLUMN	DoType  ");
                }
                #endregion

                #region 修复丢失的发起人.
                Flows fls = new Flows();
                fls.GetNewEntity.CheckPhysicsTable();

                foreach (Flow item in fls)
                {
                    if (DBAccess.IsExitsObject(item.PTable) == false)
                        continue;
                    try
                    {
                        DBAccess.RunSQL(" UPDATE " + item.PTable + " SET FlowStarter =(SELECT Starter FROM WF_GENERWORKFLOW WHERE " + item.PTable + ".Oid=WF_GENERWORKFLOW.WORKID)");
                    }
                    catch (Exception ex)
                    {
                        //   GERpt rpt=new GERpt(
                    }
                }
                #endregion 修复丢失的发起人.

                #region 给city 设置拼音.
                if (DBAccess.IsExitsObject("CN_City") == true && 1 == 2)
                {
                    if (DBAccess.IsExitsTableCol("CN_City", "PinYin") == true)
                    {
                        /*为cn_city 设置拼音.*/
                        sql = "SELECT No,Names FROM CN_City ";
                        DataTable dtCity = DBAccess.RunSQLReturnTable(sql);

                        foreach (DataRow dr in dtCity.Rows)
                        {
                            string no = dr["No"].ToString();
                            string name = dr["Names"].ToString();
                            string pinyin1 = DataType.ParseStringToPinyin(name);
                            string pinyin2 = DataType.ParseStringToPinyinJianXie(name);
                            string pinyin = "," + pinyin1 + "," + pinyin2 + ",";
                            DBAccess.RunSQL("UPDATE CN_City SET PinYin='" + pinyin + "' WHERE NO='" + no + "'");
                        }
                    }
                }
                #endregion 给city 设置拼音.

                //增加列FlowStars
                BP.WF.Port.WFEmp wfemp = new BP.WF.Port.WFEmp();
                wfemp.CheckPhysicsTable();

                DBType dbtype = BP.Difference.SystemConfig.AppCenterDBType;

                BP.Sys.FrmRB rb = new FrmRB();
                rb.CheckPhysicsTable();


                MapDtlExt dtlExt = new MapDtlExt();
                dtlExt.CheckPhysicsTable();

                //删除枚举.
                DBAccess.RunSQL("DELETE FROM " + BP.Sys.Base.Glo.SysEnum() + " WHERE EnumKey IN ('SelectorModel','CtrlWayAth')");

                //2017.5.19 打印模板字段修复
                FrmPrintTemplate bt = new FrmPrintTemplate();
                bt.CheckPhysicsTable();
                if (DBAccess.IsExitsTableCol("Sys_FrmPrintTemplate", "url") == true)
                    DBAccess.RunSQL("UPDATE Sys_FrmPrintTemplate SET TempFilePath = Url WHERE TempFilePath IS null");

                //规范升级根目录.
                DataTable dttree = DBAccess.RunSQLReturnTable("SELECT No FROM Sys_FormTree WHERE ParentNo='-1' ");
                if (dttree.Rows.Count == 1)
                {
                    DBAccess.RunSQL("UPDATE Sys_FormTree SET ParentNo='1' WHERE ParentNo='0' ");
                    DBAccess.RunSQL("UPDATE Sys_FormTree SET No='1' WHERE No='0'  ");
                    DBAccess.RunSQL("UPDATE Sys_FormTree SET ParentNo='0' WHERE No='1'");
                }

                //删除垃圾数据.
                BP.Sys.MapExt.DeleteDB();

                //升级傻瓜表单.
                MapFrmFool mff = new MapFrmFool();
                mff.CheckPhysicsTable();

                #region 表单方案中的不可编辑, 旧版本如果包含了这个列.
                if (DBAccess.IsExitsTableCol("WF_FrmNode", "IsEdit") == true)
                {
                    /*如果存在这个列,就查询出来=0的设置，就让其设置为不可以编辑的。*/
                    sql = "UPDATE WF_FrmNode SET FrmSln=1 WHERE IsEdit=0 ";
                    DBAccess.RunSQL(sql);

                    sql = "UPDATE WF_FrmNode SET IsEdit=100000";
                    DBAccess.RunSQL(sql);
                }
                #endregion

                //执行升级 2016.04.08 
                Cond cnd = new Cond();
                cnd.CheckPhysicsTable();

                #region  增加week字段,方便按周统计.
                sql = "SELECT WorkID,RDT FROM WF_GenerWorkFlow WHERE WeekNum=0 or WeekNum is null ";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    sql = "UPDATE WF_GenerWorkFlow SET WeekNum=" + DataType.CurrentWeekGetWeekByDay(dr[1].ToString().Replace("+", " ")) + " WHERE WorkID=" + dr[0].ToString();
                    DBAccess.RunSQL(sql);
                }

                //查询.
                BP.WF.Data.CH ch = new CH();
                ch.CheckPhysicsTable();

                sql = "SELECT MyPK,DTFrom FROM WF_CH WHERE WeekNum=0 or WeekNum is null ";
                dt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    sql = "UPDATE WF_CH SET WeekNum=" + DataType.CurrentWeekGetWeekByDay(dr[1].ToString()) + " WHERE MyPK='" + dr[0].ToString() + "'";
                    DBAccess.RunSQL(sql);
                }
                #endregion  增加week字段.

                #region 检查数据源.
                SFDBSrc src = new SFDBSrc();
                src.No = "local";
                src.Name = "本机数据源";
                if (src.RetrieveFromDBSources() == 0)
                    src.Insert();
                #endregion 检查数据源.

                #region 20170613.增加审核组件配置项“是否显示退回的审核信息”对应字段 by:liuxianchen
                try
                {
                    if (DBAccess.IsExitsTableCol("WF_Node", "FWCIsShowReturnMsg") == false)
                    {
                        switch (src.HisDBType)
                        {
                            case DBType.MSSQL:
                                DBAccess.RunSQL("ALTER TABLE WF_Node ADD FWCIsShowReturnMsg INT NULL");
                                break;
                            case DBType.Oracle:
                            case DBType.Informix:
                            case DBType.PostgreSQL:
                            case DBType.HGDB:
                            case DBType.UX:
                            case DBType.KingBaseR3:
                            case DBType.KingBaseR6:
                                DBAccess.RunSQL("ALTER TABLE WF_Node ADD FWCIsShowReturnMsg INTEGER NULL");
                                break;
                            case DBType.MySQL:
                                DBAccess.RunSQL("ALTER TABLE WF_Node ADD FWCIsShowReturnMsg INT NULL");
                                break;
                            default:
                                break;
                        }

                        DBAccess.RunSQL("UPDATE WF_Node SET FWCIsShowReturnMsg = 0");
                    }
                }
                catch
                {
                }
                #endregion

                #region 20170522.增加SL表单设计器中对单选/复选按钮进行字体大小调节的功能 by:liuxianchen

                FrmRB frmRB = new FrmRB();
                frmRB.CheckPhysicsTable();

                try
                {
                    DataTable columns = src.GetColumns("Sys_FrmRB");
                    if (columns.Select("No='AtPara'").Length == 0)
                    {
                        switch (src.HisDBType)
                        {
                            case DBType.MSSQL:
                                DBAccess.RunSQL("ALTER TABLE Sys_FrmRB ADD AtPara NVARCHAR(1000) NULL");
                                break;
                            case DBType.Oracle:
                            case DBType.KingBaseR3:
                            case DBType.KingBaseR6:
                                DBAccess.RunSQL("ALTER TABLE Sys_FrmRB ADD AtPara NVARCHAR2(1000) NULL");
                                break;
                            case DBType.PostgreSQL:
                            case DBType.UX:
                            case DBType.HGDB:
                                DBAccess.RunSQL("ALTER TABLE Sys_FrmRB ADD AtPara VARCHAR2(1000) NULL");
                                break;
                            case DBType.MySQL:
                            case DBType.Informix:
                                DBAccess.RunSQL("ALTER TABLE Sys_FrmRB ADD AtPara TEXT NULL");
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch
                {
                }
                #endregion

                #region 其他.
                // 更新 PassRate.
                sql = "UPDATE WF_Node SET PassRate=100 WHERE PassRate IS NULL";
                DBAccess.RunSQL(sql);
                #endregion 其他.

                #region 升级统一规则.
                #region 检查必要的升级。
                NodeWorkCheck fwc = new NodeWorkCheck();
                fwc.CheckPhysicsTable();

                Flow myfl = new Flow();
                myfl.CheckPhysicsTable();

                Node nd = new Node();
                nd.CheckPhysicsTable();

                //Sys_SFDBSrc
                SFDBSrc sfDBSrc = new SFDBSrc();
                sfDBSrc.CheckPhysicsTable();
                #endregion 检查必要的升级。
                MapExt mapExt = new MapExt();
                mapExt.CheckPhysicsTable();

                try
                {
                    string sqls = "";

                    if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle || BP.Difference.SystemConfig.AppCenterDBType == DBType.HGDB || BP.Difference.SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.UX || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR3 || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR6)
                    {
                        sqls += "UPDATE Sys_MapExt SET MyPK= ExtType||'_'||FK_Mapdata||'_'||AttrOfOper WHERE ExtType='" + MapExtXmlList.TBFullCtrl + "'";
                        sqls += "@UPDATE Sys_MapExt SET MyPK= ExtType||'_'||FK_Mapdata||'_'||AttrOfOper WHERE ExtType='" + MapExtXmlList.PopVal + "'";
                        sqls += "@UPDATE Sys_MapExt SET MyPK= ExtType||'_'||FK_Mapdata||'_'||AttrOfOper WHERE ExtType='" + MapExtXmlList.DDLFullCtrl + "'";
                        sqls += "@UPDATE Sys_MapExt SET MyPK= ExtType||'_'||FK_Mapdata||'_'||AttrsOfActive WHERE ExtType='" + MapExtXmlList.ActiveDDL + "'";
                    }
                    else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
                    {
                        sqls += "UPDATE Sys_MapExt SET MyPK=CONCAT(ExtType,'_',FK_Mapdata,'_',AttrOfOper) WHERE ExtType='" + MapExtXmlList.TBFullCtrl + "'";
                        sqls += "@UPDATE Sys_MapExt SET MyPK=CONCAT(ExtType,'_',FK_Mapdata,'_',AttrOfOper) WHERE ExtType='" + MapExtXmlList.PopVal + "'";
                        sqls += "@UPDATE Sys_MapExt SET MyPK=CONCAT(ExtType,'_',FK_Mapdata,'_',AttrOfOper) WHERE ExtType='" + MapExtXmlList.DDLFullCtrl + "'";
                        sqls += "@UPDATE Sys_MapExt SET MyPK=CONCAT(ExtType,'_',FK_Mapdata,'_',AttrOfOper) WHERE ExtType='" + MapExtXmlList.ActiveDDL + "'";
                    }
                    else
                    {
                        sqls += "UPDATE Sys_MapExt SET MyPK= ExtType+'_'+FK_Mapdata+'_'+AttrOfOper WHERE ExtType='" + MapExtXmlList.TBFullCtrl + "'";
                        sqls += "@UPDATE Sys_MapExt SET MyPK= ExtType+'_'+FK_Mapdata+'_'+AttrOfOper WHERE ExtType='" + MapExtXmlList.PopVal + "'";
                        sqls += "@UPDATE Sys_MapExt SET MyPK= ExtType+'_'+FK_Mapdata+'_'+AttrOfOper WHERE ExtType='" + MapExtXmlList.DDLFullCtrl + "'";
                        sqls += "@UPDATE Sys_MapExt SET MyPK= ExtType+'_'+FK_Mapdata+'_'+AttrsOfActive WHERE ExtType='" + MapExtXmlList.ActiveDDL + "'";

                    }

                    DBAccess.RunSQLs(sqls);
                }
                catch (Exception ex)
                {
                    BP.DA.Log.DebugWriteError(ex.Message);
                }
                #endregion

                #region 其他.
                //升级表单树. 2015.10.05
                SysFormTree sft = new SysFormTree();
                sft.CheckPhysicsTable();


                //表单信息表.
                MapDataExt mapext = new MapDataExt();
                mapext.CheckPhysicsTable();

                TransferCustom tc = new TransferCustom();
                tc.CheckPhysicsTable();

                //增加部门字段。
                CCList cc = new CCList();
                cc.CheckPhysicsTable();
                #endregion 其他.

                #region 升级sys_sftable
                //升级
                BP.Sys.SFTable tab = new BP.Sys.SFTable();
                tab.CheckPhysicsTable();
                #endregion

                #region 基础数据更新.
                Node wf_Node = new Node();
                wf_Node.CheckPhysicsTable();
                // 设置节点ICON.
                sql = "UPDATE WF_Node SET ICON='审核.png' WHERE ICON IS NULL";
                DBAccess.RunSQL(sql);

                BP.WF.Template.NodeSheet nodeSheet = new BP.WF.Template.NodeSheet();
                nodeSheet.CheckPhysicsTable();

                #endregion 基础数据更新.

                #region 升级SelectAccper

                SelectAccper selectAccper = new SelectAccper();
                selectAccper.CheckPhysicsTable();
                #endregion

                #region  升级 NodeToolbar
                FrmField ff = new FrmField();
                ff.CheckPhysicsTable();

                SysFormTree ssft = new SysFormTree();
                ssft.CheckPhysicsTable();

                BP.Sys.FrmAttachment myath = new FrmAttachment();
                myath.CheckPhysicsTable();

                FrmField ffs = new FrmField();
                ffs.CheckPhysicsTable();
                #endregion

                #region 执行sql．
                DBAccess.RunSQL("delete  from " + BP.Sys.Base.Glo.SysEnum() + " WHERE EnumKey in ('PrintFileType','EventDoType','FormType','BatchRole','StartGuideWay','NodeFormType')");
                DBAccess.RunSQL("UPDATE Sys_FrmSln SET FK_Flow =(SELECT FK_FLOW FROM WF_Node WHERE NODEID=Sys_FrmSln.FK_Node) WHERE FK_Flow IS NULL");

                if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MSSQL)
                    DBAccess.RunSQL("UPDATE WF_FrmNode SET MyPK=FK_Frm+'_'+convert(varchar,FK_Node )+'_'+FK_Flow");

                if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle || BP.Difference.SystemConfig.AppCenterDBType == DBType.HGDB || BP.Difference.SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.UX || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR3 || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR6)
                    DBAccess.RunSQL("UPDATE WF_FrmNode SET MyPK=FK_Frm||'_'||FK_Node||'_'||FK_Flow");

                if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
                    DBAccess.RunSQL("UPDATE WF_FrmNode SET MyPK=CONCAT(FK_Frm,'_',FK_Node,'_',FK_Flow)");

                #endregion

                #region 执行更新.wf_node
                sql = "UPDATE WF_Node SET FWCType=0 WHERE FWCType IS NULL";
                sql += "@UPDATE WF_Node SET FWC_H=0 WHERE FWC_H IS NULL";
                DBAccess.RunSQLs(sql);

                sql = "UPDATE WF_Node SET SFSta=0 WHERE SFSta IS NULL";
                sql += "@UPDATE WF_Node SET SF_H=0 WHERE SF_H IS NULL";
                DBAccess.RunSQLs(sql);
                #endregion 执行更新.

                #region 升级站内消息表 2013-10-20
                BP.WF.SMS sms = new SMS();
                sms.CheckPhysicsTable();
                #endregion


                #region 升级Img
                FrmImg img = new FrmImg();
                img.CheckPhysicsTable();

                ExtImg myimg = new ExtImg();
                myimg.CheckPhysicsTable();
                if (DBAccess.IsExitsTableCol("Sys_FrmImg", "SrcType") == true)
                {
                    DBAccess.RunSQL("UPDATE Sys_FrmImg SET ImgSrcType = SrcType WHERE ImgSrcType IS NULL");
                    DBAccess.RunSQL("UPDATE Sys_FrmImg SET ImgSrcType = 0 WHERE ImgSrcType IS NULL");
                }
                #endregion

                #region 修复 mapattr UIHeight, UIWidth 类型错误.
                switch (BP.Difference.SystemConfig.AppCenterDBType)
                {
                    case DBType.Oracle:
                    case DBType.KingBaseR3:
                    case DBType.KingBaseR6:
                        msg = "@Sys_MapAttr 修改字段";
                        break;
                    case DBType.MSSQL:
                        msg = "@修改sql server控件高度和宽度字段。";
                        DBAccess.RunSQL("ALTER TABLE Sys_MapAttr ALTER COLUMN UIWidth float");
                        DBAccess.RunSQL("ALTER TABLE Sys_MapAttr ALTER COLUMN UIHeight float");
                        break;
                    default:
                        break;
                }
                #endregion

                #region 升级常用词汇
                switch (BP.Difference.SystemConfig.AppCenterDBType)
                {
                    case DBType.Oracle:
                    case DBType.KingBaseR3:
                    case DBType.KingBaseR6:
                        int i = DBAccess.RunSQLReturnCOUNT("SELECT * FROM USER_TAB_COLUMNS WHERE TABLE_NAME = 'SYS_DEFVAL' AND COLUMN_NAME = 'PARENTNO'");
                        if (i == 0)
                        {
                            if (DBAccess.IsExitsObject("SYS_DEFVAL") == true)
                                DBAccess.RunSQL("drop table Sys_DefVal");

                            DefVal dv = new DefVal();
                            dv.CheckPhysicsTable();
                        }
                        msg = "@Sys_DefVal 修改字段";
                        break;
                    case DBType.MSSQL:
                        msg = "@修改 Sys_DefVal。";
                        i = DBAccess.RunSQLReturnCOUNT("SELECT * FROM SYSCOLUMNS WHERE ID=OBJECT_ID('Sys_DefVal') AND NAME='ParentNo'");
                        if (i == 0)
                        {
                            if (DBAccess.IsExitsObject("Sys_DefVal") == true)
                                DBAccess.RunSQL("drop table Sys_DefVal");

                            DefVal dv = new DefVal();
                            dv.CheckPhysicsTable();
                        }
                        break;
                    default:
                        break;
                }
                #endregion

                #region 登陆更新错误
                msg = "@登陆时错误。";
                DBAccess.RunSQL("DELETE FROM " + BP.Sys.Base.Glo.SysEnum() + " WHERE EnumKey IN ('DeliveryWay','RunModel','OutTimeDeal','FlowAppType')");
                #endregion 登陆更新错误

                #region 升级表单树
                // 首先检查是否升级过.
                //sql = "SELECT * FROM Sys_FormTree WHERE No = '1'";
                //DataTable formTree_dt = DBAccess.RunSQLReturnTable(sql);
                //if (formTree_dt.Rows.Count == 0)
                //{
                //    /*没有升级过.增加根节点*/
                //    SysFormTree formTree = new SysFormTree();
                //    formTree.No = "1";
                //    formTree.Name = "表单库";
                //    formTree.ParentNo = "0";
                //    // formTree.TreeNo = "0";
                //    formTree.Idx = 0;
                //    formTree.IsDir = true;
                //        formTree.DirectInsert();

                //    //将表单库中的数据转入表单树
                //    SysFormTrees formSorts = new SysFormTrees();
                //    formSorts.RetrieveAll();

                //    foreach (SysFormTree item in formSorts)
                //    {
                //        if (item.No == "0")
                //            continue;
                //        SysFormTree subFormTree = new SysFormTree();
                //        subFormTree.No = item.No;
                //        subFormTree.Name = item.Name;
                //        subFormTree.ParentNo = "0";
                //        subFormTree.Save();
                //    }
                //    //表单于表单树进行关联
                //    sql = "UPDATE Sys_MapData SET FK_FormTree=FK_FrmSort WHERE FK_FrmSort <> '' AND FK_FrmSort IS not null";
                //    DBAccess.RunSQL(sql);
                //}
                #endregion

                #region 执行admin登陆. 2012-12-25 新版本.
                Emp emp = new Emp("admin");
                if (emp.RetrieveFromDBSources() == 1)
                {
                    BP.Web.WebUser.SignInOfGener(emp);
                }
                else
                {
                    emp.SetValByKey("No", "admin");
                    emp.Name = "admin";
                    emp.DeptNo = "01";
                    emp.Pass = "123";
                    emp.Insert();
                    BP.Web.WebUser.SignInOfGener(emp);
                    //throw new Exception("admin 用户丢失，请注意大小写。");
                }
                #endregion 执行admin登陆.

                #region 修复 Sys_FrmImg 表字段 ImgAppType Tag0
                switch (BP.Difference.SystemConfig.AppCenterDBType)
                {
                    case DBType.Oracle:
                    case DBType.KingBaseR3:
                    case DBType.KingBaseR6:
                        int i = DBAccess.RunSQLReturnCOUNT("SELECT * FROM USER_TAB_COLUMNS WHERE TABLE_NAME = 'SYS_FRMIMG' AND COLUMN_NAME = 'TAG0'");
                        if (i == 0)
                        {
                            DBAccess.RunSQL("ALTER TABLE SYS_FRMIMG ADD (ImgAppType number,TAG0 nvarchar(500))");
                        }
                        msg = "@Sys_FrmImg 修改字段";
                        break;
                    case DBType.MSSQL:
                        msg = "@修改 Sys_FrmImg。";
                        i = DBAccess.RunSQLReturnCOUNT("SELECT * FROM SYSCOLUMNS WHERE ID=OBJECT_ID('Sys_FrmImg') AND NAME='Tag0'");
                        if (i == 0)
                        {
                            DBAccess.RunSQL("ALTER TABLE Sys_FrmImg ADD ImgAppType int");
                            DBAccess.RunSQL("ALTER TABLE Sys_FrmImg ADD Tag0 nvarchar(500)");
                        }
                        break;
                    default:
                        break;
                }
                #endregion

                #region 20161101.升级表单，增加图片附件必填验证 by:liuxianchen

                BP.Sys.FrmImgAth imgAth = new BP.Sys.FrmImgAth();
                imgAth.CheckPhysicsTable();

                sql = "UPDATE Sys_FrmImgAth SET IsRequired = 0 WHERE IsRequired IS NULL";
                DBAccess.RunSQL(sql);
                #endregion

                #region 20170520.附件删除规则修复
                try
                {
                    DataTable columns = src.GetColumns("Sys_FrmAttachment");
                    if (columns.Select("No='DeleteWay'").Length > 0 && columns.Select("No='IsDelete'").Length > 0)
                    {
                        DBAccess.RunSQL("UPDATE SYS_FRMATTACHMENT SET DeleteWay=IsDelete WHERE DeleteWay IS NULL");
                    }
                }
                catch
                {
                }
                #endregion

                #region 密码加密
                if (BP.Difference.SystemConfig.isEnablePasswordEncryption == true && DBAccess.IsView("Port_Emp", BP.Difference.SystemConfig.AppCenterDBType) == false)
                {
                    BP.Port.Emps emps = new BP.Port.Emps();
                    emps.RetrieveAllFromDBSource();
                    foreach (Emp empEn in emps)
                    {
                        if (string.IsNullOrEmpty(empEn.Pass) || empEn.Pass.Length < 30)
                        {
                            empEn.Pass = BP.Tools.Cryptography.MD5_Encrypt(empEn.Pass);
                            empEn.DirectUpdate();
                        }
                    }
                }
                #endregion

                // 最后更新版本号，然后返回.
                sql = "UPDATE Sys_Serial SET IntVal=" + Ver + " WHERE CfgKey='Ver'";
                if (DBAccess.RunSQL(sql) == 0)
                {
                    sql = "INSERT INTO Sys_Serial (CfgKey,IntVal) VALUES('Ver'," + Ver + ") ";
                    DBAccess.RunSQL(sql);
                }
                // 返回版本号.
                return "旧版本:(" + currDBVer + ")新版本:(" + Ver + ")"; // +"\t\n解决问题:" + updataNote;
            }
            catch (Exception ex)
            {
                string err = "问题出处:" + ex.Message + "<hr>" + msg + "<br>详细信息:@" + ex.StackTrace + "<br>@<a href='../DBInstall.htm' >点这里到系统升级界面。</a>";
                BP.DA.Log.DebugWriteError("系统升级错误:" + err);
                return err;
            }
        }
        /// <summary>
        /// 如果发现升级sql文件日期变化了，就自动升级.
        /// 就是说该文件如果被修改了就会自动升级.
        /// </summary>
        public static void UpdataCCFlowVerSQLScript()
        {

            string sql = "SELECT IntVal FROM Sys_Serial WHERE CfgKey='UpdataCCFlowVer'";
            string currDBVer = DBAccess.RunSQLReturnStringIsNull(sql, "");

            string sqlScript = BP.Difference.SystemConfig.PathOfData + "UpdataCCFlowVer.sql";
            System.IO.FileInfo fi = new System.IO.FileInfo(sqlScript);
            if (File.Exists(sqlScript) == false)
                return;
            string myVer = fi.LastWriteTime.ToString("yyyyMMddHH");

            //判断是否可以执行，当文件发生变化后，才执行。
            if (currDBVer == "" || int.Parse(currDBVer) < int.Parse(myVer))
            {
                DBAccess.RunSQLScript(sqlScript);
                sql = "UPDATE Sys_Serial SET IntVal=" + myVer + " WHERE CfgKey='UpdataCCFlowVer'";

                if (DBAccess.RunSQL(sql) == 0)
                {
                    sql = "INSERT INTO Sys_Serial (CfgKey,IntVal) VALUES('UpdataCCFlowVer'," + myVer + ") ";
                    DBAccess.RunSQL(sql);
                }
            }
        }
        /// <summary>
        /// CCFlowAppPath
        /// </summary>
        public static string CCFlowAppPath
        {
            get
            {
                return BP.Difference.SystemConfig.GetValByKey("CCFlowAppPath", "/");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool IsEnableHuiQianList
        {
            get
            {
                if (BP.Difference.SystemConfig.CustomerNo == "TianYe")
                    return true;

                return BP.Difference.SystemConfig.GetValByKeyBoolen("IsEnableHuiQianList", false);
            }
        }
        /// <summary>
        /// 检查是否可以安装驰骋BPM系统
        /// </summary>
        /// <returns></returns>
        public static bool IsCanInstall()
        {
            string sql = "";
            string errInfo = "";
            try
            {
                errInfo = " 当前用户没有[查询系统表]的权限. ";

                if (DBAccess.IsExitsObject("AA"))
                {
                    errInfo = " 当前用户没有[删除表]的权限. ";
                    sql = "DROP TABLE AA";
                    DBAccess.RunSQL(sql);
                }

                errInfo = " 当前用户没有[创建表]的权限. ";
                sql = "CREATE TABLE AA (OID int NOT NULL)"; //检查是否可以创建表.
                DBAccess.RunSQL(sql);

                errInfo = " 当前用户没有[插入数据]的权限. ";
                sql = "INSERT INTO AA (OID) VALUES(100)";
                DBAccess.RunSQL(sql);

                try
                {
                    //检查是否可以批量执行sql.
                    sql = "UPDATE AA SET OID=0 WHERE OID=1;UPDATE AA SET OID=0 WHERE OID=1;";
                    DBAccess.RunSQL(sql);
                }
                catch
                {
                    throw new Exception("err@需要让数据库链接支持批量执行SQL语句，请修改数据库链接配置：&allowMultiQueries=true");
                }

                errInfo = " 当前用户没有[update 表数据]的权限. ";
                sql = "UPDATE AA SET OID=101";
                DBAccess.RunSQL(sql);

                errInfo = " 当前用户没有[delete 表数据]的权限. ";
                sql = "DELETE FROM AA";
                DBAccess.RunSQL(sql);

                errInfo = " 当前用户没有[创建表主键]的权限. ";
                DBAccess.CreatePK("AA", "OID", BP.Difference.SystemConfig.AppCenterDBType);

                errInfo = " 当前用户没有[创建索引]的权限. ";
                DBAccess.CreatIndex("AA", "OID"); //可否创建索引.

                errInfo = " 当前用户没有[查询数据表]的权限. ";
                sql = "select * from AA "; //检查是否有查询的权限.
                DBAccess.RunSQLReturnTable(sql);

                errInfo = " 当前数据库设置区分了大小写，不能对大小写敏感，如果是mysql数据库请参考 https://blog.csdn.net/ccflow/article/details/100079825 ";
                sql = "select * from aa "; //检查是否区分大小写.
                DBAccess.RunSQLReturnTable(sql);

                if (DBAccess.IsExitsObject("AAVIEW"))
                {
                    errInfo = " 当前用户没有[删除视图]的权限. ";
                    sql = "DROP VIEW AAVIEW";
                    DBAccess.RunSQL(sql);
                }

                errInfo = " 当前用户没有[创建视图]的权限.";
                sql = "CREATE VIEW AAVIEW AS SELECT * FROM AA "; //检查是否可以创建视图.
                DBAccess.RunSQL(sql);

                errInfo = " 当前用户没有[删除视图]的权限.";
                sql = "DROP VIEW AAVIEW"; //检查是否可以删除视图.
                DBAccess.RunSQL(sql);

                errInfo = " 当前用户没有[删除表]的权限.";
                sql = "DROP TABLE AA"; //检查是否可以删除表.
                DBAccess.RunSQL(sql);

                if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
                {
                    try
                    {
                        sql = " set @@global.sql_mode ='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';";
                        DBAccess.RunSQL(sql);
                    }
                    catch
                    {
                    }
                }

                if (BP.Difference.SystemConfig.AppCenterDBDatabase.Contains("-") == true)
                    throw new Exception("err@数据库名称不能包含 '-' 号，您可以使用 '_' .");

                return true;
            }
            catch (Exception ex)
            {
                if (DBAccess.IsExitsObject("AA") == true)
                {
                    sql = "DROP TABLE AA";
                    DBAccess.RunSQL(sql);
                }

                if (DBAccess.IsExitsObject("AAVIEW") == true)
                {
                    sql = "DROP VIEW AAVIEW";
                    DBAccess.RunSQL(sql);
                }

                string info = "驰骋工作流引擎 - 检查数据库安装权限出现错误:";
                info += "\t\n1. 当前登录的数据库帐号，必须有创建、删除视图或者table的权限。";
                info += "\t\n2. 必须对表有增、删、改、查的权限。 ";
                info += "\t\n3. 必须有删除创建索引主键的权限。 ";
                info += "\t\n4. 我们建议您设置当前数据库连接用户为管理员权限。 ";
                info += "\t\n ccbpm检查出来的信息如下：" + errInfo;
                info += "\t\n etc: 数据库测试异常信息:" + ex.Message;
                throw new Exception("err@" + info);
                //  return false;
            }
            return true;
        }

        /// <summary>
        /// 安装包
        /// </summary>
        /// <param name="lang">语言</param>
        /// <param name="demoType">0安装Demo, 1 不安装Demo.</param>
        public static void DoInstallDataBase(string lang, int demoType)
        {
            bool isInstallFlowDemo = true;
            if (demoType == 2)
                isInstallFlowDemo = false;

            #region 首先创建Port类型的表, 让admin登录.

            SFDBSrc sf = new SFDBSrc();
            sf.CheckPhysicsTable();

            FrmRB rb = new FrmRB();
            rb.CheckPhysicsTable();

            BP.Port.Emp portEmp = new BP.Port.Emp();
            portEmp.CheckPhysicsTable();


            BP.Port.Dept mydept = new BP.Port.Dept();
            mydept.CheckPhysicsTable();

            Station mySta = new Station();
            mySta.CheckPhysicsTable();

            StationType myStaType = new StationType();
            myStaType.CheckPhysicsTable();

            BP.Port.DeptEmp myde = new BP.Port.DeptEmp();
            myde.CheckPhysicsTable();

            BP.Port.DeptEmpStation mydes = new BP.Port.DeptEmpStation();
            mydes.CheckPhysicsTable();

            BP.WF.Port.WFEmp wfemp = new BP.WF.Port.WFEmp();
            wfemp.CheckPhysicsTable();

            
            if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
            {
                BP.WF.Port.AdminGroup.Org org = new Port.AdminGroup.Org();
                org.CheckPhysicsTable();

                BP.WF.Port.Admin2Group.OrgAdminer oa = new Port.Admin2Group.OrgAdminer();
                oa.CheckPhysicsTable();

                BP.WF.Template.FlowSort fs = new FlowSort();
                fs.CheckPhysicsTable();

                BP.WF.Template.SysFormTree ft = new SysFormTree();
                ft.CheckPhysicsTable();
            }

            if (DBAccess.IsExitsTableCol("WF_Emp", "StartFlows") == false)
            {
                string sql = "";
                //增加StartFlows这个字段
                switch (BP.Difference.SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                        sql = "ALTER TABLE WF_Emp ADD StartFlows Text DEFAULT  NULL";
                        break;
                    case DBType.Oracle:
                    case DBType.KingBaseR3:
                    case DBType.KingBaseR6:
                        sql = "ALTER TABLE  WF_EMP add StartFlows BLOB";
                        break;
                    case DBType.MySQL:
                        sql = "ALTER TABLE WF_Emp ADD StartFlows TEXT COMMENT '可以发起的流程'";
                        break;
                    case DBType.Informix:
                        sql = "ALTER TABLE WF_Emp ADD StartFlows VARCHAR(4000) DEFAULT  NULL";
                        break;
                    case DBType.PostgreSQL:
                    case DBType.UX:
                    case DBType.HGDB:
                        sql = "ALTER TABLE WF_Emp ADD StartFlows Text DEFAULT  NULL";
                        break;
                    default:
                        throw new Exception("@没有涉及到的数据库类型");
                }
                DBAccess.RunSQL(sql);
            }
            #endregion 首先创建Port类型的表.

            #region 3, 执行基本的 sql
            string sqlscript = "";

            BP.Port.Emp empGPM = new BP.Port.Emp();
            empGPM.CheckPhysicsTable();

            BP.Port.DeptEmp de = new BP.Port.DeptEmp();
            de.CheckPhysicsTable();


            if (DBAccess.IsExitsTableCol("Port_Emp", "Pass") == false)
            {
                if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle)
                    DBAccess.RunSQL("ALTER TABLE Port_Emp ADD Pass VARCHAR(90) DEFAULT '123' NULL ");
                else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.HGDB)
                    DBAccess.RunSQL("ALTER TABLE Port_Emp ADD Pass VARCHAR(90) DEFAULT '123' NULL ");
                else
                    DBAccess.RunSQL("ALTER TABLE Port_Emp ADD Pass NVARCHAR(90) DEFAULT '123' NULL ");
            }
            //初始化数据
            sqlscript = BP.Difference.SystemConfig.CCFlowAppPath + "WF/Data/Install/SQLScript/Port_Inc_CH_RunModel_" + (int)SystemConfig.CCBPMRunModel + ".sql";
            DBAccess.RunSQLScript(sqlscript);


            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
            {
                BP.Port.Emp empAdmin = new Emp("admin");
                BP.Web.WebUser.SignInOfGener(empAdmin);
            }

            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                BP.WF.Dev2Interface.Port_Login("admin", "100", null);
            }

            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.GroupInc)
            {
                BP.WF.Dev2Interface.Port_Login("admin", "100", null);
            }
            #endregion 执行基本的 sql

            ArrayList al = null;
            string info = "BP.En.Entity";
            al = BP.En.ClassFactory.GetObjects(info);

            #region 先创建表，否则列的顺序就会变化.
            FlowExt fe = new FlowExt();
            fe.CheckPhysicsTable();

            NodeExt ne = new NodeExt();
            ne.CheckPhysicsTable();

            MapDtl mapdtl = new MapDtl();
            mapdtl.CheckPhysicsTable();

            MapData mapData = new MapData();
            mapData.CheckPhysicsTable();

            SysEnum sysenum = new SysEnum();
            sysenum.CheckPhysicsTable();

            BP.WF.Template.NodeWorkCheck fwc = new NodeWorkCheck();
            fwc.CheckPhysicsTable();

            MapAttr attr = new MapAttr();
            attr.CheckPhysicsTable();

            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.CheckPhysicsTable();

            BP.WF.Data.CH ch = new CH();
            ch.CheckPhysicsTable();

            #endregion 先创建表，否则列的顺序就会变化.

            #region 1, 创建or修复表
            foreach (Object obj in al)
            {
                Entity en = null;
                en = obj as Entity;
                if (en == null)
                    continue;

                //获得类名.
                string clsName = en.ToString();
                if (DataType.IsNullOrEmpty(clsName) == true)
                    continue;

                if (clsName.Contains("FlowSheet") == true)
                    continue;
                if (clsName.Contains("NodeSheet") == true)
                    continue;
                if (clsName.Contains("FlowFormTree") == true)
                    continue;

                //不安装CCIM的表.
                if (clsName.Contains("BP.CCIM"))
                    continue;

                //抽象的类不允许创建表.
                switch (clsName.ToUpper())
                {
                    case "BP.WF.STARTWORK":
                    case "BP.WF.WORK":
                    case "BP.WF.GESTARTWORK":
                    case "BP.EN.GENONAME":
                    case "BP.EN.GETREE":
                    case "BP.WF.GERpt":
                    case "BP.WF.GEENTITY":
                    case "BP.SYS.TSENTITYNONAME":
                        continue;
                    default:
                        break;
                }

                if (isInstallFlowDemo == false)
                {
                    /*如果不安装demo.*/
                    if (clsName.Contains("BP.CN")
                        || clsName.Contains("BP.Demo"))
                        continue;
                }

                string table = null;
                try
                {
                    table = en.EnMap.PhysicsTable;
                    if (table == null)
                        continue;
                }
                catch
                {
                    continue;
                }

                try
                {
                    switch (table)
                    {
                        case "WF_EmpWorks":
                        case "WF_GenerEmpWorkDtls":
                        case "WF_GenerEmpWorks":
                            continue;
                        case "Sys_Enum":
                            en.CheckPhysicsTable();
                            break;
                        default:
                            en.CheckPhysicsTable();
                            break;
                    }
                    en.CheckPhysicsTable();
                }
                catch
                {
                }
            }
            #endregion 修复

            #region 2, 注册枚举类型 SQL
            // 2, 注册枚举类型。
            BP.Sys.XML.EnumInfoXmls xmls = new BP.Sys.XML.EnumInfoXmls();
            xmls.RetrieveAll();
            foreach (BP.Sys.XML.EnumInfoXml xml in xmls)
            {
                BP.Sys.SysEnums ses = new BP.Sys.SysEnums();
                int count = ses.RetrieveByAttr(SysEnumAttr.EnumKey, xml.Key);
                if (count > 0)
                    continue;
                ses.RegIt(xml.Key, xml.Vals);
            }
            #endregion 注册枚举类型

            #region 2.1 重新构建视图. //删除视图.
            ReCreateView();
            #endregion 创建视图与数据

            #region 5, 初始化数据.
            if (isInstallFlowDemo)
            {
                // sqlscript =  BP.Difference.SystemConfig.PathOfData + "Install/SQLScript/InitPublicData.sql";
                // DBAccess.RunSQLScript(sqlscript);
            }
            // else
            // {
            // FlowSort fs = new FlowSort();
            // fs.No = "1";
            // fs.ParentNo = "0";
            // fs.Name = "流程树";
            // fs.DirectInsert();
            // }
            #endregion 初始化数据

            #region 6, 生成临时的 wf_emp 数据。
            if (isInstallFlowDemo == true)
            {
                BP.Port.Emps emps = new BP.Port.Emps();
                emps.RetrieveAllFromDBSource();
                int i = 0;
                foreach (BP.Port.Emp emp in emps)
                {
                    i++;
                    BP.WF.Port.WFEmp wfEmp = new BP.WF.Port.WFEmp();
                    wfEmp.Copy(emp);
                    wfEmp.No = emp.UserID;

                    /* if (wfEmp.Email.Length == 0)
                         wfEmp.Email = emp.UserID + "@ccbpm.cn";*/

                    if (wfEmp.Tel.Length == 0)
                        wfEmp.Tel = "82374939-6" + i.ToString().PadLeft(2, '0');

                    if (wfEmp.IsExits)
                        wfEmp.Update();
                    else
                        wfEmp.Insert();
                }

                // 生成简历数据.
                foreach (BP.Port.Emp emp in emps)
                {
                    for (int myIdx = 0; myIdx < 6; myIdx++)
                    {
                        string sql = "";
                        sql = "INSERT INTO Demo_Resume (OID,RefPK,NianYue,GongZuoDanWei,ZhengMingRen,BeiZhu,QT) ";
                        sql += "VALUES(" + DBAccess.GenerOID("Demo_Resume") + ",'" + emp.UserID + "','200" + myIdx + "-01','成都.驰骋" + myIdx + "公司','张三','表现良好','其他-" + myIdx + "无')";
                        DBAccess.RunSQL(sql);
                    }
                }

                DataTable dtStudent = DBAccess.RunSQLReturnTable("SELECT No FROM Demo_Student");
                foreach (DataRow dr in dtStudent.Rows)
                {
                    string no = dr[0].ToString();
                    for (int myIdx = 0; myIdx < 6; myIdx++)
                    {
                        string sql = "";
                        sql = "INSERT INTO Demo_Resume (OID,RefPK,NianYue,GongZuoDanWei,ZhengMingRen,BeiZhu,QT) ";
                        sql += "VALUES(" + DBAccess.GenerOID("Demo_Resume") + ",'" + no + "','200" + myIdx + "-01','成都.驰骋" + myIdx + "公司','张三','表现良好','其他-" + myIdx + "无')";
                        DBAccess.RunSQL(sql);
                    }
                }
                GenerWorkFlowViewNY ny = new GenerWorkFlowViewNY();
                ny.CheckPhysicsTable();
                // 生成年度月份数据.
                string sqls = "";
                DateTime dtNow = DateTime.Now;
                for (int num = 0; num < 12; num++)
                {
                    sqls = "@ INSERT INTO Pub_NY (No,Name) VALUES ('" + dtNow.ToString("yyyy-MM") + "','" + dtNow.ToString("yyyy-MM") + "')  ";
                    dtNow = dtNow.AddMonths(1);
                }
                DBAccess.RunSQLs(sqls);
            }
            #endregion 初始化数据

            #region 7, 装载 demo.flow
            if (isInstallFlowDemo == true && SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
            {
                BP.Port.Emp emp = new BP.Port.Emp("admin");
                BP.Web.WebUser.SignInOfGener(emp);
                BP.Sys.Base.Glo.WriteLineInfo("开始装载模板...");
                string msg = "";

                //装载数据模版.
                BP.WF.DTS.LoadTemplete l = new BP.WF.DTS.LoadTemplete();
                msg = l.Do() as string;
            }
            
            if (isInstallFlowDemo == false && SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
            {
                //创建一个空白的流程，不然，整个结构就出问题。
                //    FlowSorts fss = new FlowSorts();
                // fss.RetrieveAll();
                //  fss.Delete();
                DBAccess.RunSQL("DELETE FROM WF_FlowSort ");
                FlowSort fs = new FlowSort();
                fs.Name = "流程树";
                fs.No = "1";
                fs.ParentNo = "0";
                fs.Insert();

                fs = new FlowSort();
                fs.No = "101";
                fs.ParentNo = "1";
                fs.Name = "日常办公类";
                fs.Insert();

                //加载一个模版,不然用户不知道如何新建流程.
                BP.WF.Template.TemplateGlo.LoadFlowTemplate(fs.No, BP.Difference.SystemConfig.PathOfData + "Install/QingJiaFlowDemoInit.xml",
                    ImpFlowTempleteModel.AsTempleteFlowNo);

                Flow fl = new Flow("001");
                fl.DoCheck(); //做一下检查.

                fs.No = "102";
                fs.ParentNo = "1";
                fs.Name = "财务类";
                fs.Insert();

                fs.No = "103";
                fs.ParentNo = "1";
                fs.Name = "人力资源类";
                fs.Insert();

                DBAccess.RunSQL("DELETE FROM Sys_FormTree ");
                SysFormTree ftree = new SysFormTree();
                ftree.Name = "表单树";
                ftree.No = "1";
                ftree.ParentNo = "0";
                ftree.Insert();

                SysFormTree subFrmTree = new SysFormTree();
                subFrmTree.Name = "流程独立表单";
                subFrmTree.ParentNo = "1";
                subFrmTree.No = "101";
                subFrmTree.Insert();

                subFrmTree = new SysFormTree(); ;
                subFrmTree.No = "102";
                subFrmTree.Name = "常用信息管理";
                subFrmTree.ParentNo = "1";
                subFrmTree.Insert();

                subFrmTree = new SysFormTree(); ;
                subFrmTree.Name = "常用单据";
                subFrmTree.No = "103";
                subFrmTree.ParentNo = "1";
                subFrmTree.Insert();
            }
            #endregion 装载demo.flow

            #region 增加图片签名
            if (isInstallFlowDemo == true)
            {
                try
                {
                    //增加图片签名
                    BP.WF.DTS.GenerSiganture gs = new BP.WF.DTS.GenerSiganture();
                    gs.Do();
                }
                catch
                {
                }
            }
            #endregion 增加图片签名.

            //生成拼音，以方便关键字查找.
            BP.WF.DTS.GenerPinYinForEmp pinyin = new BP.WF.DTS.GenerPinYinForEmp();
            pinyin.Do();

            #region 执行补充的sql, 让外键的字段长度都设置成100.
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET maxlen=100 WHERE LGType=2 AND MaxLen<100");
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET maxlen=100 WHERE KeyOfEn='FK_Dept'");
            #endregion 执行补充的sql, 让外键的字段长度都设置成100.

            #region 如果是第一次运行，就执行检查。
            if (isInstallFlowDemo == true)
            {
                Flows fls = new Flows();
                fls.RetrieveAllFromDBSource();
                foreach (Flow fl in fls)
                {
                    try
                    {
                        fl.DoCheck();
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DebugWriteError(ex.Message);
                    }
                }
            }
            #endregion 如果是第一次运行，就执行检查。

            #region 增加大文本字段列.
            try
            {
                if (DBAccess.IsExitsTableCol("Sys_MapData", "HtmlTemplateFile") == false)
                {
                    string sql = "ALTER TABLE Sys_MapData ADD  HtmlTemplateFile TEXT ";
                    DBAccess.RunSQL(sql);
                }
                DBAccess.GetBigTextFromDB("Sys_MapData", "No", "001", "HtmlTemplateFile");
            }
            catch(Exception e)
            {
                throw new Exception("err@增加大文本字段列时出现异常;@异常信息:" + e.Message);
            }
            #endregion 增加大文本字段列.

            #region 检查工作处理器的ShiftTo和ShiftNote字段是否存在
            try
            {
                if (DBAccess.IsExitsTableCol("WF_WorkOpt", "ShiftTo") == false)
                {
                    string sql = "ALTER TABLE WF_WorkOpt ADD  ShiftTo VARCHAR(100) COMMENT '接收人'";
                    DBAccess.RunSQL(sql);
                }
                if (DBAccess.IsExitsTableCol("WF_WorkOpt", "ShiftNote") == false)
                {
                    string sql = "ALTER TABLE WF_WorkOpt ADD  ShiftNote  text COMMENT '移交原因'";
                    DBAccess.RunSQL(sql);
                }
            }
            catch (Exception e)
            {
                throw new Exception("err@检查ShiftTo和ShiftNote字段时出现异常;@异常信息:" + e.Message);
            }
            #endregion 检查工作处理器的ShiftTo和ShiftNote字段是否存在

            #region 检查菜单表的UrlPath，path，Alias，IframeOpenType等字段是否存在
            try
            {
                if (DBAccess.IsExitsTableCol("GPM_Menu", "UrlPath") == false)
                {
                    string sql = "ALTER TABLE GPM_Menu ADD  UrlPath VARCHAR(500) COMMENT 'Vue文件路径'";
                    DBAccess.RunSQL(sql);
                }
                if (DBAccess.IsExitsTableCol("GPM_Menu", "path") == false)
                {
                    string sql = "ALTER TABLE GPM_Menu ADD  path VARCHAR(50) COMMENT 'path'";
                    DBAccess.RunSQL(sql);
                }
                if (DBAccess.IsExitsTableCol("GPM_Menu", "Alias") == false)
                {
                    string sql = "ALTER TABLE GPM_Menu ADD  Alias VARCHAR(500) COMMENT '别名'";
                    DBAccess.RunSQL(sql);
                }
                if (DBAccess.IsExitsTableCol("GPM_Menu", "IframeOpenType") == false)
                {
                    string sql = "ALTER TABLE GPM_Menu ADD  IframeOpenType VARCHAR(50) COMMENT 'iframe打开方式'";
                    DBAccess.RunSQL(sql);
                }
                if (DBAccess.IsExitsTableCol("GPM_Menu", "IframeOpenTypeT") == false)
                {
                    string sql = "ALTER TABLE GPM_Menu ADD  IframeOpenTypeT VARCHAR(200) COMMENT 'iframe打开方式'";
                    DBAccess.RunSQL(sql);
                }
                if (DBAccess.IsExitsTableCol("GPM_Menu", "IframeOpenTypeT") == false)
                {
                    string sql = "ALTER TABLE GPM_Menu ADD  IframeOpenTypeT VARCHAR(200) COMMENT 'iframe打开方式'";
                    DBAccess.RunSQL(sql);
                }
                if (DBAccess.IsExitsTableCol("GPM_Menu", "ModuleNoT") == false)
                {
                    string sql = "ALTER TABLE GPM_Menu ADD  ModuleNoT VARCHAR(200) COMMENT '隶属模块编号'";
                    DBAccess.RunSQL(sql);
                } 
                if (DBAccess.IsExitsTableCol("GPM_Menu", "EnName") == false)
                {
                    string sql = "ALTER TABLE GPM_Menu ADD  EnName VARCHAR(300) COMMENT '对应的实体类'";
                    DBAccess.RunSQL(sql);
                }
            }
            catch (Exception e)
            {
                throw new Exception("err@检查菜单表的字段时出现异常;@异常信息:" + e.Message);
            }
            #endregion 检查菜单表的UrlPath，path，Alias，IframeOpenType等字段是否存在

            #region 检查表单树的字段是否存在
            try
            {
                if (DBAccess.IsExitsTableCol("Sys_FormTree", "ShortName") == false)
                {
                    string sql = "ALTER TABLE Sys_FormTree ADD  ShortName VARCHAR(200) COMMENT '简称'";
                    DBAccess.RunSQL(sql);
                }
                if (DBAccess.IsExitsTableCol("Sys_FormTree", "Domain") == false)
                {
                    string sql = "ALTER TABLE Sys_FormTree ADD  Domain  VARCHAR(100) COMMENT '域/系统编号'";
                    DBAccess.RunSQL(sql);
                }
            }
            catch (Exception e)
            {
                throw new Exception("err@检查表单树的字段时出现异常;@异常信息:" + e.Message);
            }

            #endregion 检查表单树的字段是否存在
            
            #region 检查流程类别表的字段是否存在
            try
            {
                if (DBAccess.IsExitsTableCol("WF_FlowSort", "DirType") == false)
                {
                    string sql = "ALTER TABLE WF_FlowSort ADD  DirType Int(11) COMMENT '目录类型'";
                    DBAccess.RunSQL(sql);
                }
            }
            catch (Exception e)
            {
                throw new Exception("err@检查流程类别表的字段时出现异常;@异常信息:" + e.Message);
            }
            #endregion 检查流程类别表的字段是否存在

            #region 检查表单共享、流程共享字段是否存在
            try
            {
                //流程共享
                if (DBAccess.IsExitsTableCol("WF_Flow", "ShareSln") == false)
                {
                    string sql = "ALTER TABLE WF_Flow ADD  ShareSln Int(11) COMMENT '流程共享方案'";
                    DBAccess.RunSQL(sql);
                }
                //表单共享
                if (DBAccess.IsExitsTableCol("Sys_MapData", "ShareSln") == false)
                {
                    string sql = "ALTER TABLE Sys_MapData ADD  ShareSln Int(11) COMMENT '表单共享方案'";
                    DBAccess.RunSQL(sql);
                }
                //显示列
                if (DBAccess.IsExitsTableCol("Sys_MapData", "ShowColModel") == false)
                {
                    string sql = "ALTER TABLE Sys_MapData ADD  ShowColModel Int(11) COMMENT '显示列'";
                    DBAccess.RunSQL(sql);
                }
            }
            catch (Exception e)
            {
                throw new Exception("err@检查表单共享、流程共享字段时出现异常;@异常信息:" + e.Message);
            }
            #endregion 检查表单共享、流程共享字段是否存在

            #region 检查SQL、JS、TS字段是否存在
            try
            {
                //SQL脚本内容
                if (DBAccess.IsExitsTableCol("Frm_Method", "SQLScript") == false)
                {
                    string sql = "ALTER TABLE Frm_Method ADD  SQLScript TEXT COMMENT 'SQL脚本内容'";
                    DBAccess.RunSQL(sql);
                }
                //JS脚本内容
                if (DBAccess.IsExitsTableCol("Frm_Method", "JSScript") == false)
                {
                    string sql = "ALTER TABLE Frm_Method ADD  JSScript TEXT COMMENT 'JS脚本内容'";
                    DBAccess.RunSQL(sql);
                }
                //TS脚本内容
                if (DBAccess.IsExitsTableCol("Frm_Method", "TSScript") == false)
                {
                    string sql = "ALTER TABLE Frm_Method ADD  TSScript TEXT COMMENT 'TS脚本内容'";
                    DBAccess.RunSQL(sql);
                }
                //执行按钮
                if (DBAccess.IsExitsTableCol("Frm_Method", "BtnDoneText") == false)
                {
                    string sql = "ALTER TABLE Frm_Method ADD  BtnDoneText VARCHAR(100) COMMENT '执行按钮'";
                    DBAccess.RunSQL(sql);
                }
            }
            catch (Exception e)
            {
                throw new Exception("err@检查检查SQL、JS、TS字段时出现异常;@异常信息:" + e.Message);
            }
            #endregion 检查SQL、JS、TS字段是否存在

            #region 检查WF_Node
            try
            {
                if (DBAccess.IsExitsTableCol("WF_Node", "CancelNodes") == false)
                {
                    string sql = "ALTER TABLE WF_Node ADD  CancelNodes VARCHAR(200) COMMENT '可撤销的节点'";
                    DBAccess.RunSQL(sql);
                }
            }
            catch (Exception e)
            {
                throw new Exception("err@检查WF_Node的字段时出现异常;@异常信息:" + e.Message);
            }

            #endregion 检查WF_Node

            #region 处理密码加密.
            string pass = SystemConfig.UserDefaultPass;
            if (SystemConfig.isEnablePasswordEncryption)
                pass = BP.Tools.Cryptography.MD5_Encrypt(pass);
            DBAccess.RunSQL("UPDATE Port_Emp SET Pass='" + pass + "'");
            #endregion 处理密码加密.
        }

        public static void ReCreateView()
        {
            //抄送规则.
            BP.WF.Template.CCRole cc = new CCRole();
            cc.CheckPhysicsTable();

            BP.WF.CCList list = new CCList();
            list.CheckPhysicsTable();

            if (DBAccess.IsExitsTableCol("WF_GenerWorkerList", "FK_NodeText") == true)
            {
                DBAccess.RenameTableField("WF_GenerWorkerList", "FK_NodeText", "NodeName");
                DBAccess.RenameTableField("WF_GenerWorkerList", "FK_EmpTex", "EmpName");
                DBAccess.RenameTableField("WF_GenerWorkerList", "FK_DeptT", "DeptName");
            }

            if (DBAccess.IsExitsObject("WF_EmpWorks") == true)
                DBAccess.RunSQL("DROP VIEW WF_EmpWorks");

            if (DBAccess.IsExitsObject("V_WF_Delay") == true)
                DBAccess.RunSQL("DROP VIEW V_WF_Delay");

            if (DBAccess.IsExitsObject("V_FlowStarter") == true)
                DBAccess.RunSQL("DROP VIEW V_FlowStarter");

            if (DBAccess.IsExitsObject("V_FlowStarterBPM") == true)
                DBAccess.RunSQL("DROP VIEW V_FlowStarterBPM");

            if (DBAccess.IsExitsObject("V_TOTALCH") == true)
                DBAccess.RunSQL("DROP VIEW V_TOTALCH");

            if (DBAccess.IsExitsObject("V_TOTALCHYF") == true)
                DBAccess.RunSQL("DROP VIEW V_TOTALCHYF");

            if (DBAccess.IsExitsObject("V_TotalCHWeek") == true)
                DBAccess.RunSQL("DROP VIEW V_TotalCHWeek");

            string prix = "";
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                prix = "";

            //执行必须的sql.
            string sqlscript = "";
            //执行必须的sql.
            switch (BP.Difference.SystemConfig.AppCenterDBType)
            {
                case DBType.Oracle:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    sqlscript = BP.Difference.SystemConfig.CCFlowAppPath + "WF/Data/Install/SQLScript/InitView_Ora" + prix + ".sql";
                    break;
                case DBType.MSSQL:
                case DBType.Informix:
                    sqlscript = BP.Difference.SystemConfig.CCFlowAppPath + "WF/Data/Install/SQLScript/InitView_SQL" + prix + ".sql";
                    break;
                case DBType.MySQL:
                    sqlscript = BP.Difference.SystemConfig.CCFlowAppPath + "WF/Data/Install/SQLScript/InitView_MySQL" + prix + ".sql";
                    break;
                case DBType.PostgreSQL:
                case DBType.UX:
                case DBType.HGDB:
                    sqlscript = BP.Difference.SystemConfig.CCFlowAppPath + "WF/Data/Install/SQLScript/InitView_PostgreSQL" + prix + ".sql";
                    break;
                default:
                    break;
            }
            DBAccess.RunSQLScript(sqlscript, false);
        }

        public static void KillProcess(string processName) //杀掉进程的方法
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process pro in processes)
            {
                string name = pro.ProcessName + ".exe";
                if (name.ToLower() == processName.ToLower())
                    pro.Kill();
            }
        }
        #endregion 执行安装.

        #region 流程模版的ftp服务器配置.
        public static string TemplateFTPHost
        {
            get
            {
                return BP.Difference.SystemConfig.GetValByKey("TemplateFTPHost", "116.239.32.14");
            }
        }
        public static int TemplateFTPPort
        {
            get
            {
                return BP.Difference.SystemConfig.GetValByKeyInt("TemplateFTPPort", 9997);
            }
        }
        public static string TemplateFTPUser
        {
            get
            {
                return BP.Difference.SystemConfig.GetValByKey("TemplateFTPUser", "oa");
            }
        }
        public static string TemplateFTPPassword
        {
            get
            {
                return BP.Difference.SystemConfig.GetValByKey("TemplateFTPPassword", "Jszx1234");
            }
        }
        #endregion 流程模版的ftp服务器配置.

        #region 全局的方法处理
        /// <summary>
        /// 流程数据表系统字段,中间用,分开.
        /// </summary>
        public static string FlowFields
        {
            get
            {
                string str = "";
                str += GERptAttr.OID + ",";
                str += GERptAttr.AtPara + ",";
                str += GERptAttr.BillNo + ",";
                //  str += GERptAttr.CFlowNo + ",";
                //  str += GERptAttr.CWorkID + ",";
                str += GERptAttr.FID + ",";
                str += GERptAttr.FK_Dept + ",";
                str += GERptAttr.FK_DeptName + ",";
                str += GERptAttr.FK_NY + ",";
                str += GERptAttr.FlowDaySpan + ",";
                str += GERptAttr.FlowEmps + ",";
                str += GERptAttr.FlowEnder + ",";
                str += GERptAttr.FlowEnderRDT + ",";
                str += GERptAttr.FlowEndNode + ",";
                str += GERptAttr.FlowStarter + ",";
                str += GERptAttr.FlowStartRDT + ",";
                str += GERptAttr.GuestName + ",";
                str += GERptAttr.GuestNo + ",";
                str += GERptAttr.GUID + ",";
                str += GERptAttr.PEmp + ",";
                str += GERptAttr.PFID + ",";
                str += GERptAttr.PFlowNo + ",";
                str += GERptAttr.PNodeID + ",";
                str += GERptAttr.PrjName + ",";
                str += GERptAttr.PrjNo + ",";
                str += GERptAttr.PWorkID + ",";
                str += GERptAttr.Title + ",";
                str += GERptAttr.WFSta + ",";
                str += GERptAttr.WFState + ",";
                str += "Rec,";
                str += "CDT,RDT,WFStateText";
                return str;
            }
        }
        #endregion 全局的方法处理

        #region 与流程事件实体相关.
        private static Hashtable Htable_FlowFEE = null;
        /// <summary>
        /// 获得节点事件实体
        /// </summary>
        /// <param name="enName">实例名称</param>
        /// <returns>获得节点事件实体,如果没有就返回为空.</returns>
        public static FlowEventBase GetFlowEventEntityByEnName(string enName)
        {
            if (Htable_FlowFEE == null || Htable_FlowFEE.Count == 0)
            {
                Htable_FlowFEE = new Hashtable();

                string name = "BP.WF.FlowEventBase";

                ArrayList al = BP.En.ClassFactory.GetObjects(name);
                foreach (FlowEventBase en in al)
                {
                    if (Htable_FlowFEE.ContainsKey(en.ToString()) == true)
                        continue;
                    Htable_FlowFEE.Add(en.ToString(), en);
                }
            }
            FlowEventBase myen = Htable_FlowFEE[enName] as FlowEventBase;
            if (myen == null)
            {
                //throw new Exception("@根据类名称获取流程事件实体实例出现错误:" + enName + ",没有找到该类的实体.");
                BP.DA.Log.DebugWriteError("@根据类名称获取流程事件实体实例出现错误:" + enName + ",没有找到该类的实体.");
                return null;
            }
            return myen;
        }
        /// <summary>
        /// 获得事件实体String，根据编号或者流程标记
        /// </summary>
        /// <param name="flowMark">流程标记</param>
        /// <param name="flowNo">流程编号</param>
        /// <returns>null, 或者流程实体.</returns>
        public static string GetFlowEventEntityStringByFlowMark(string flowMark)
        {
            FlowEventBase en = GetFlowEventEntityByFlowMark(flowMark);
            if (en == null)
                return "";
            return en.ToString();
        }
        /// <summary>
        /// 获得事件实体，根据编号或者流程标记.
        /// </summary>
        /// <param name="flowMark">流程标记</param>
        /// <param name="flowNo">流程编号</param>
        /// <returns>null, 或者流程实体.</returns>
        public static FlowEventBase GetFlowEventEntityByFlowMark(string flowMark)
        {

            if (Htable_FlowFEE == null || Htable_FlowFEE.Count == 0)
            {
                Htable_FlowFEE = new Hashtable();

                string name = "";
                name = "BP.WF.FlowEventBase";

                ArrayList al = BP.En.ClassFactory.GetObjects(name);
                Htable_FlowFEE.Clear();
                foreach (FlowEventBase en in al)
                {
                    if (Htable_FlowFEE.Contains(en.ToString()) == true)
                        continue;
                    Htable_FlowFEE.Add(en.ToString(), en);
                }
            }

            foreach (string key in Htable_FlowFEE.Keys)
            {
                FlowEventBase fee = Htable_FlowFEE[key] as FlowEventBase;

                string mark = "," + fee.FlowMark + ",";
                if (mark.Contains("," + flowMark + ",") == true)
                    return fee;
            }
            return null;
        }
        #endregion 与流程事件实体相关.

        #region web.config 属性.
        public static bool IsEnableTrackRec
        {
            get
            {
                string s = BP.Difference.SystemConfig.AppSettings["IsEnableTrackRec"];
                if (string.IsNullOrEmpty(s))
                    return false;
                if (s == "0")
                    return false;

                return true;
            }
        }
        public static string MapDataLikeKey(string flowNo, string colName)
        {
            flowNo = int.Parse(flowNo).ToString();
            string len = BP.Difference.SystemConfig.AppCenterDBLengthStr;

            //edited by liuxc,2016-02-22,合并逻辑，原来分流程编号的位数，现在统一处理
            return " (" + colName + " LIKE 'ND" + flowNo + "%' AND " + len + "(" + colName + ")=" +
                   ("ND".Length + flowNo.Length + 2) + ") OR (" + colName +
                   " = 'ND" + flowNo + "Rpt' ) OR (" + colName + " LIKE 'ND" + flowNo + "__Dtl%' AND " + len + "(" +
                   colName + ")>" + ("ND".Length + flowNo.Length + 2 + "Dtl".Length) + ")";
        }

        #endregion webconfig属性.

        #region 常用方法
        /// <summary>
        /// 加入track
        /// </summary>
        /// <param name="at">事件类型</param>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="fid">流程ID</param>
        /// <param name="fromNodeID">从节点编号</param>
        /// <param name="fromNodeName">从节点名称</param>
        /// <param name="fromEmpID">从人员ID</param>
        /// <param name="fromEmpName">从人员名称</param>
        /// <param name="toNodeID">到节点编号</param>
        /// <param name="toNodeName">到节点名称</param>
        /// <param name="toEmpID">到人员ID</param>
        /// <param name="toEmpName">到人员名称</param>
        /// <param name="note">消息</param>
        /// <param name="tag">参数用@分开</param>
        public static string AddToTrack(ActionType at, string flowNo, Int64 workID, Int64 fid, int fromNodeID, string fromNodeName, string fromEmpID, string fromEmpName,
            int toNodeID, string toNodeName, string toEmpID, string toEmpName, string note, string tag)
        {
            if (toNodeID == 0)
            {
                toNodeID = fromNodeID;
                toNodeName = fromNodeName;
            }

            Track t = new Track();
            t.FlowNo = flowNo;
            t.WorkID = workID;
            t.FID = fid;
            t.RDT = DataType.CurrentDateTimess;
            t.HisActionType = at;

            t.NDFrom = fromNodeID;
            t.NDFromT = fromNodeName;

            t.EmpFrom = fromEmpID;
            t.EmpFromT = fromEmpName;

            t.NDTo = toNodeID;
            t.NDToT = toNodeName;
            t.FlowNo = flowNo;

            string[] empNos = toEmpID.Split(',');
            if (empNos.Length <= 100)
            {
                t.EmpTo = toEmpID;
                t.EmpToT = toEmpName;
            }
            else
            {
                string[] empNames = toEmpName.Split('、');
                //获取
                t.EmpTo = string.Join(",", empNos.Take(100)) + "..." + empNos[empNos.Length - 1];
                t.EmpToT = string.Join("'、", empNames.Take(100)) + "..." + empNames[empNames.Length - 1];
            }

            t.Msg = note;
            t.NodeData = "@DeptNo=" + WebUser.DeptNo + "@DeptName=" + WebUser.DeptName;
            //参数.
            if (tag != null)
                t.Tag = tag;

            try
            {
                t.Insert();
            }
            catch
            {
                t.CheckPhysicsTable();
                t.Insert();
            }
            return t.MyPK;
        }

        /// <summary>
        /// SQL表达式是否正确
        /// </summary>
        /// <param name="sqlExp"></param>
        /// <param name="ht"></param>
        /// <returns></returns>
        public static bool CondExpSQL(string sqlExp, Hashtable ht, Int64 myWorkID)
        {
            string sql = sqlExp;
            sql = sql.Replace("~", "'");
            sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
            sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
            sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.DeptNo);

            foreach (string key in ht.Keys)
            {
                if (key == "OID")
                {
                    sql = sql.Replace("@WorkID", ht["OID"].ToString());
                    sql = sql.Replace("@OID", ht["OID"].ToString());
                    continue;
                }
                sql = sql.Replace("@" + key, ht[key].ToString());
            }

            //从工作流参数里面替换
            if (sql.Contains("@") == true && myWorkID != 0)
            {
                GenerWorkFlow gwf = new GenerWorkFlow(myWorkID);
                AtPara ap = gwf.atPara;
                foreach (string str in ap.HisHT.Keys)
                {
                    sql = sql.Replace("@" + str, ap.GetValStrByKey(str));
                }
            }

            int result = DBAccess.RunSQLReturnValInt(sql, -1);
            if (result <= 0)
                return false;
            return true;
        }
        /// <summary>
        /// 判断表达式是否成立
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <param name="en">变量</param>
        /// <returns>是否成立</returns>
        public static bool CondExpPara(string exp, Hashtable ht, Int64 myWorkID)
        {
            try
            {
                string[] strs = exp.Trim().Split(' ');

                string key = strs[0].Trim();
                string oper = strs[1].Trim();
                string val = strs[2].Trim();

                val = val.Replace("'", "");
                val = val.Replace("%", "");
                val = val.Replace("~", "");

                string valPara = null;
                if (ht.ContainsKey(key) == false)
                {

                    bool isHave = false;
                    if (myWorkID != 0)
                    {
                        //把外部传来的参数传入到 rptGE 让其做方向条件的判断.
                        GenerWorkFlow gwf = new GenerWorkFlow(myWorkID);
                        AtPara at = gwf.atPara;
                        foreach (string str in at.HisHT.Keys)
                        {
                            if (key.Equals(str) == false)
                                continue;

                            valPara = at.GetValStrByKey(key);
                            isHave = true;
                            break;
                        }
                    }

                    if (isHave == false)
                    {
                        try
                        {
                            if (BP.Difference.SystemConfig.isBSsystem == true && HttpContextHelper.RequestParamKeys.Contains(key) == true)
                                valPara = HttpContextHelper.RequestParams(key);
                            else
                                throw new Exception("@判断条件时错误,请确认参数是否拼写错误,没有找到对应的表达式:" + exp + " Key=(" + key + ") oper=(" + oper + ")Val=(" + val + ")");
                        }
                        catch
                        {
                            //有可能是常量. 
                            valPara = key;
                        }
                    }
                }
                else
                {
                    valPara = ht[key].ToString().Trim();
                }

                #region 开始执行判断.
                if (oper == "=")
                {
                    if (valPara == val)
                        return true;
                    else
                        return false;
                }

                if (oper.ToUpper() == "LIKE")
                {
                    if (valPara.Contains(val))
                        return true;
                    else
                        return false;
                }
                if (oper == "!=")
                {
                    if (valPara != val)
                        return true;
                    else
                        return false;
                }


                if (DataType.IsNumStr(valPara) == false)
                    throw new Exception("err@表达式错误:[" + exp + "]没有找到参数[" + valPara + "]的值，导致无法计算。");

                if (oper == ">")
                {
                    if (float.Parse(valPara) > float.Parse(val))
                        return true;
                    else
                        return false;
                }
                if (oper == ">=")
                {
                    if (float.Parse(valPara) >= float.Parse(val))
                        return true;
                    else
                        return false;
                }
                if (oper == "<")
                {
                    if (float.Parse(valPara) < float.Parse(val))
                        return true;
                    else
                        return false;
                }
                if (oper == "<=")
                {
                    if (float.Parse(valPara) <= float.Parse(val))
                        return true;
                    else
                        return false;
                }

                if (oper == "!=")
                {
                    if (float.Parse(valPara) != float.Parse(val))
                        return true;
                    else
                        return false;
                }
                throw new Exception("@参数格式错误:" + exp + " Key=" + key + " oper=" + oper + " Val=" + val);
                #endregion 开始执行判断.

            }
            catch (Exception ex)
            {
                throw new Exception("计算参数的时候出现错误:" + ex.Message);
            }
        }
        /// <summary>
        /// 表达式替换
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string DealExp(string exp, Entity en, string errInfo = "")
        {
            //替换字符
            exp = exp.Replace("~~", "\"");
            exp = exp.Replace("~", "'");

            if (exp.Contains("@") == false)
                return exp;

            //首先替换加; 的。
            exp = exp.Replace("@WebUser.No;", WebUser.No);
            exp = exp.Replace("@WebUser.Name;", WebUser.Name);
            exp = exp.Replace("@WebUser.FK_DeptName;", WebUser.DeptName);
            exp = exp.Replace("@WebUser.FK_Dept;", WebUser.DeptNo);

            // 替换没有 ; 的 .
            exp = exp.Replace("@WebUser.No", WebUser.No);
            exp = exp.Replace("@WebUser.Name", WebUser.Name);
            exp = exp.Replace("@WebUser.FK_DeptName", WebUser.DeptName);
            exp = exp.Replace("@WebUser.FK_Dept", WebUser.DeptNo);
            exp = exp.Replace("@WebUser.OrgNo", WebUser.OrgNo);

            exp = exp.Replace("@RDT", DataType.CurrentDateTime);

            if (exp.Contains("@") == false)
                return exp;

            //增加对新规则的支持. @MyField; 格式.
            if (en != null)
            {
                Attrs attrs = en.EnMap.Attrs;
                BP.En.Row row = en.Row;
                //特殊判断.
                if (row.ContainsKey("OID") == true)
                    exp = exp.Replace("@WorkID", row["OID"].ToString());

                if (exp.Contains("@") == false)
                    return exp;


                bool isHaveFenHao = exp.Contains(';');


                foreach (string key in row.Keys)
                {
                    //值为空或者null不替换
                    if (row[key] == null)
                        continue;
                    if (exp.Contains("@" + key + ";"))
                    {
                        //先替换有单引号的.
                        exp = exp.Replace("'@" + key + ";'", "'" + row[key].ToString() + "'");
                        //在更新没有单引号的.
                        exp = exp.Replace("@" + key + ";", row[key].ToString());
                    }
                    if (exp.Contains("@" + key))
                    {
                        //先替换有单引号的.
                        exp = exp.Replace("'@" + key + "'", "'" + row[key].ToString() + "'");
                        //在更新没有单引号的.
                        exp = exp.Replace("@" + key, row[key].ToString());
                    }
                    //不包含@则返回SQL语句
                    if (exp.Contains("@") == false)
                        return exp;
                }
            }

            if (exp.Contains("@") && BP.Difference.SystemConfig.isBSsystem == true)
            {
                /*如果是bs*/
                foreach (string key in HttpContextHelper.RequestParamKeys)
                {
                    if (string.IsNullOrEmpty(key))
                        continue;
                    exp = exp.Replace("@" + key, HttpContextHelper.RequestParams(key));
                }

            }

            exp = exp.Replace("~", "'");
            return exp;
        }
        //
        /// <summary>
        /// 处理表达式
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <param name="en">数据源</param>
        /// <param name="errInfo">错误</param>
        /// <returns></returns>
        public static string DealSQLExp(string exp, Entity en, string errInfo)
        {
            //替换字符
            exp = exp.Replace("~~", "\"");
            exp = exp.Replace("~", "'");

            //替换我们只处理WHERE 后面的内容
            //需要判断SQL 中含有几个WHERE字符
            int num = Regex.Matches(exp.ToUpper(), "WHERE").Count;
            if (num == 0)
                return exp;
            //我们暂时处理含有一个WHERE的情况
            string expFrom = "";
            if (num == 1)
            {
                expFrom = exp.Substring(0, exp.ToUpper().IndexOf("WHERE"));
                exp = exp.Substring(expFrom.Length);
            }

            string expWhere = "";

            if (exp.Contains("@") == false)
                return expFrom + exp;

            //首先替换加; 的。
            exp = exp.Replace("@WebUser.No;", WebUser.No);
            exp = exp.Replace("@WebUser.Name;", WebUser.Name);
            exp = exp.Replace("@WebUser.FK_DeptName;", WebUser.DeptName);
            exp = exp.Replace("@WebUser.FK_Dept;", WebUser.DeptNo);


            // 替换没有 ; 的 .
            exp = exp.Replace("@WebUser.No", WebUser.No);
            exp = exp.Replace("@WebUser.Name", WebUser.Name);
            exp = exp.Replace("@WebUser.FK_DeptName", WebUser.DeptName);
            exp = exp.Replace("@WebUser.FK_Dept", WebUser.DeptNo);

            if (Glo.CCBPMRunModel != CCBPMRunModel.Single)
                exp = exp.Replace("@WebUser.OrgNo", WebUser.OrgNo);

            if (exp.Contains("@") == false)
                return expFrom + exp;

            //增加对新规则的支持. @MyField; 格式.
            if (en != null)
            {
                BP.En.Row row = en.Row;
                //特殊判断.
                if (row.ContainsKey("OID") == true)
                    exp = exp.Replace("@WorkID", row["OID"].ToString());

                if (exp.Contains("@") == false)
                    return expFrom + exp;

                foreach (string key in row.Keys)
                {
                    //值为空或者null不替换
                    if (row[key] == null || row[key].Equals("") == true)
                        continue;

                    if (exp.Contains("@" + key + ";"))
                        exp = exp.Replace("@" + key + ";", row[key].ToString());

                    //不包含@则返回SQL语句
                    if (exp.Contains("@") == false)
                        return expFrom + exp;
                }


                #region 解决排序问题.
                Attrs attrs = en.EnMap.Attrs;
                string mystrs = "";
                foreach (Attr attr in attrs)
                {
                    if (attr.MyDataType == DataType.AppString)
                        mystrs += "@" + attr.Key + ",";
                    else
                        mystrs += "@" + attr.Key;
                }
                string[] strs = mystrs.Split('@');
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("No", typeof(string)));
                foreach (string str in strs)
                {
                    if (string.IsNullOrEmpty(str))
                        continue;

                    DataRow dr = dt.NewRow();
                    dr[0] = str;
                    dt.Rows.Add(dr);
                }
                DataView dv = dt.DefaultView;
                dv.Sort = "No DESC";
                DataTable dtNew = dv.Table;
                #endregion  解决排序问题.

                #region 替换变量.
                foreach (DataRow dr in dtNew.Rows)
                {
                    string key = dr[0].ToString();
                    bool isStr = key.Contains(",");
                    if (isStr == true)
                        key = key.Replace(",", "");

                    if (DataType.IsNullOrEmpty(en.GetValStrByKey(key)))
                        continue;

                    exp = exp.Replace("@" + key, en.GetValStrByKey(key));
                }
                #endregion

                if (exp.Contains("@") == false)
                    return expFrom + exp;
            }

            if (exp.Contains("@") && BP.Difference.SystemConfig.isBSsystem == true)
            {
                /*如果是bs*/
                foreach (string key in HttpContextHelper.RequestParamKeys)
                {
                    if (string.IsNullOrEmpty(key))
                        continue;
                    exp = exp.Replace("@" + key, HttpContextHelper.RequestParams(key));
                }
            }
            exp = exp.Replace("~", "'");
            exp = exp.Replace("\r", "");
            exp = exp.Replace("\n", "");
            return expFrom + exp;
        }
        /// <summary>
        /// 加密MD5
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GenerMD5(BP.WF.Work wk)
        {
            string s = null;
            foreach (Attr attr in wk.EnMap.Attrs)
            {
                switch (attr.Key)
                {
                    case WorkAttr.MD5:
                    case WorkAttr.Rec:
                    case GERptAttr.Title:
                    // case GERptAttr.Emps:
                    case GERptAttr.FK_Dept:
                    //case GERptAttr.PRI:
                    case GERptAttr.FID:
                        continue;
                    default:
                        break;
                }

                string obj = attr.DefaultVal as string;
                //if (obj == null)
                //    continue;
                if (obj != null && obj.Contains("@"))
                    continue;

                s += wk.GetValStrByKey(attr.Key);
            }
            s += "ccflow";

            return GetMD5Hash(s);
        }
        /// <summary>
        /// 取得MD5加密串
        /// </summary>
        /// <param name="input">源明文字符串</param>
        /// <returns>密文字符串</returns>
        public static string GetMD5Hash(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = md5.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }
        #endregion 常用方法

        #region 其他配置.
        public static string FlowFileBill
        {
            get { return Glo.IntallPath + "DataUser/Bill/"; }
        }
        private static string _IntallPath = null;
        public static string IntallPath
        {
            get
            {
                if (_IntallPath == null)
                {
                    if (BP.Difference.SystemConfig.isBSsystem == true)
                        _IntallPath = BP.Difference.SystemConfig.PathOfWebApp;
                }

                if (_IntallPath == null)
                    throw new Exception("@没有实现如何获得 cs 下的根目录.");

                return _IntallPath;
            }
            set
            {
                _IntallPath = value;
            }
        }
        /// <summary>
        /// 语言
        /// </summary>
        public static string Language = "CH";
        /// <summary>
        /// 是否启用共享任务池？
        /// </summary>
        public static bool IsEnableTaskPool
        {
            get
            {
                return BP.Difference.SystemConfig.GetValByKeyBoolen("IsEnableTaskPool", false);
            }
        }
        /// <summary>
        /// 用户信息显示格式
        /// </summary>
        public static UserInfoShowModel UserInfoShowModel
        {
            get
            {
                return (UserInfoShowModel)SystemConfig.GetValByKeyInt("UserInfoShowModel", 0);
            }
        }
        /// <summary>
        /// 处理显示格式
        /// </summary>
        /// <param name="no"></param>
        /// <param name="name"></param>
        /// <returns>现实格式</returns>
        public static string DealUserInfoShowModel(string no, string name)
        {
            switch (BP.WF.Glo.UserInfoShowModel)
            {
                case UserInfoShowModel.UserIDOnly:
                    return no;
                case UserInfoShowModel.UserIDUserName:
                    // return "(" + no + "," + name + ")";
                    return no + "," + name;
                case UserInfoShowModel.UserNameOnly:
                    //return "(" + name + ")";
                    return name;
                default:
                    throw new Exception("@没有判断的格式类型.");
                    break;
            }
        }
        /// <summary>
        /// 钉钉是否启用
        /// </summary>
        public static bool IsEnable_DingDing
        {
            get
            {
                //如果两个参数都不为空说明启用
                string corpid = BP.Difference.SystemConfig.Ding_CorpID;
                string corpsecret = BP.Difference.SystemConfig.Ding_CorpSecret;
                if (string.IsNullOrEmpty(corpid) || string.IsNullOrEmpty(corpsecret))
                    return false;

                return true;
            }
        }
        /// <summary>
        /// 微信是否启用
        /// </summary>
        public static bool IsEnable_WeiXin
        {
            get
            {
                //如果两个参数都不为空说明启用
                string corpid = BP.Difference.SystemConfig.WX_CorpID;
                string corpsecret = BP.Difference.SystemConfig.WX_AppSecret;
                if (string.IsNullOrEmpty(corpid) || string.IsNullOrEmpty(corpsecret))
                    return false;

                return true;
            }
        }
        /// <summary>
        /// 是否启用消息系统消息。
        /// </summary>
        public static bool IsEnableSysMessage
        {
            get
            {
                return BP.Difference.SystemConfig.GetValByKeyBoolen("IsEnableSysMessage", true);
            }
        }
        /// <summary>
        /// 主机
        /// </summary>
        public static string HostURL
        {
            get
            {
                if (BP.Difference.SystemConfig.isBSsystem)
                {
                    /* 如果是BS 就要求 路径.*/
                }

                string baseUrl = BP.Difference.SystemConfig.AppSettings["HostURL"];
                if (string.IsNullOrEmpty(baseUrl) == true)
                    baseUrl = "http://127.0.0.1/";

                if (baseUrl.Substring(baseUrl.Length - 1) != "/")
                    baseUrl = baseUrl + "/";
                return baseUrl;
            }
        }
        #endregion

        #region 时间计算.
        /// <summary>
        /// 设置成工作时间
        /// </summary>
        /// <param name="DateTime"></param>
        /// <returns></returns>
        public static DateTime SetToWorkTime(DateTime dt)
        {
            if (BP.Sys.GloVar.Holidays.Contains(dt.ToString("MM-dd")))
            {
                dt = dt.AddDays(1);
                /*如果当前是节假日，就要从下一个有效期计算。*/
                while (true)
                {
                    if (BP.Sys.GloVar.Holidays.Contains(dt.ToString("MM-dd")) == false)
                        break;
                    dt = dt.AddDays(1);
                }

                //从下一个上班时间计算.
                dt = DataType.ParseSysDate2DateTime(dt.ToString("yyyy-MM-dd") + " " + Glo.AMFrom);
                return dt;
            }

            int timeInt = int.Parse(dt.ToString("HHmm"));

            //判断是否在A区间, 如果是，就返回A区间的时间点.
            if (Glo.AMFromInt >= timeInt)
                return DataType.ParseSysDate2DateTime(dt.ToString("yyyy-MM-dd") + " " + Glo.AMFrom);


            //判断是否在E区间, 如果是就返回第2天的上班时间点.
            if (Glo.PMToInt <= timeInt)
            {
                return DataType.ParseSysDate2DateTime(dt.ToString("yyyy-MM-dd") + " " + Glo.PMTo);
            }

            //如果在午休时间点中间.
            if (Glo.AMToInt <= timeInt && Glo.PMFromInt > timeInt)
            {
                return DataType.ParseSysDate2DateTime(dt.ToString("yyyy-MM-dd") + " " + Glo.PMFrom);
            }
            return dt;
        }
        /// <summary>
        /// 在指定的日期上增加小时数。
        /// 1，扣除午休。
        /// 2，扣除节假日。
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="hours"></param>
        /// <returns></returns>
        private static DateTime AddMinutes(DateTime dt, int hh, int minutes)
        {
            if (1 == 1)
            {
                dt = dt.AddHours(hh);
                dt = dt.AddMinutes(minutes);
                return dt;
            }

            //如果没有设置,就返回.
            if (minutes == 0 && hh == 0)
            {
                return dt;
            }

            //设置成工作时间.
            dt = SetToWorkTime(dt);

            //首先判断是否是在一天整的时间完成.
            if (minutes == Glo.AMPMHours * 60)
            {
                /*如果需要在一天完成*/
                dt = DataType.AddDays(dt, 1, TWay.Holiday);
                return dt;
            }

            //判断是否是AM.
            bool isAM = false;
            int timeInt = int.Parse(dt.ToString("HHmm"));
            if (Glo.AMToInt > timeInt)
                isAM = true;

            #region 如果是当天的情况.
            //如果规定的时间在 1天之内.
            if (minutes / 60 / Glo.AMPMHours < 1)
            {
                if (isAM == true)
                {
                    /*如果是中午, 中午到中午休息之间的时间. */

                    TimeSpan ts = DataType.ParseSysDateTime2DateTime(dt.ToString("yyyy-MM-dd") + " " + Glo.AMTo) - dt;
                    if (ts.TotalMinutes >= minutes)
                    {
                        /*如果剩余的分钟大于 要增加的分钟数，就是说+上分钟后，仍然在中午，就直接增加上这个分钟，让其返回。*/
                        return dt.AddMinutes(minutes);
                    }
                    else
                    {
                        // 求出到下班时间的分钟数。
                        TimeSpan myts = DataType.ParseSysDateTime2DateTime(dt.ToString("yyyy-MM-dd") + " " + Glo.PMTo) - dt;

                        // 扣除午休的时间.
                        int leftMuit = (int)(myts.TotalMinutes - Glo.AMPMTimeSpan * 60);
                        if (leftMuit - minutes >= 0)
                        {
                            /*说明还是在当天的时间内.*/
                            DateTime mydt = DataType.ParseSysDateTime2DateTime(dt.ToString("yyyy-MM-dd") + " " + Glo.PMTo);
                            return mydt.AddMinutes(minutes - leftMuit);
                        }

                        //说明要跨到第2天上去了.
                        dt = DataType.AddDays(dt, 1, TWay.Holiday);
                        return Glo.AddMinutes(dt.ToString("yyyy-MM-dd") + " " + Glo.AMFrom, minutes - leftMuit);
                    }

                    // 把当前的时间加上去.
                    dt = dt.AddMinutes(minutes);

                    //判断是否是中午.
                    bool isInAM = false;
                    timeInt = int.Parse(dt.ToString("HHmm"));
                    if (Glo.AMToInt >= timeInt)
                        isInAM = true;

                    if (isInAM == true)
                    {
                        // 加上时间后仍然是中午就返回.
                        return dt;
                    }

                    //延迟一个午休时间.
                    dt = dt.AddHours(Glo.AMPMTimeSpan);

                    //判断时间点是否落入了E区间.
                    timeInt = int.Parse(dt.ToString("HHmm"));
                    if (Glo.PMToInt <= timeInt)
                    {
                        /*如果落入了E区间.*/

                        // 求出来时间点到，下班之间的分钟数.
                        TimeSpan tsE = dt - DataType.ParseSysDate2DateTime(dt.ToString("yyyy-MM-dd") + " " + Glo.PMTo);

                        //从次日的上班时间计算+ 这个时间差. 
                        dt = DataType.ParseSysDate2DateTime(dt.ToString("yyyy-MM-dd") + " " + Glo.PMTo);
                        return dt.AddMinutes(tsE.TotalMinutes);
                    }
                    else
                    {
                        /*过了第2天的情况很少，就不考虑了.*/
                        return dt;
                    }
                }
                else
                {
                    /*如果是下午, 计算出来到下午下班还需多少分钟，与增加的分钟数据相比较. */
                    TimeSpan ts = DataType.ParseSysDateTime2DateTime(dt.ToString("yyyy-MM-dd") + " " + Glo.PMTo) - dt;
                    if (ts.TotalMinutes >= minutes)
                    {
                        /*如果剩余的分钟大于 要增加的分钟数，就直接增加上这个分钟，让其返回。*/
                        return dt.AddMinutes(minutes);
                    }
                    else
                    {

                        //剩余的分钟数 = 总分钟数 - 今天下午剩余的分钟数.
                        int leftMin = minutes - (int)ts.TotalMinutes;

                        /*否则要计算到第2天上去了， 计算时间要从下一个有效的工作日上班时间开始. */
                        dt = DataType.AddDays(DataType.ParseSysDateTime2DateTime(dt.ToString("yyyy-MM-dd") + " " + Glo.AMFrom), 1, TWay.Holiday);

                        //递归调用,让其在次日的上班时间在增加，分钟数。
                        return Glo.AddMinutes(dt, 0, leftMin);
                    }

                }
            }
            #endregion 如果是当天的情况.

            return dt;
        }
        /// <summary>
        /// 增加分钟数.
        /// </summary>
        /// <param name="sysdt"></param>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static DateTime AddMinutes(string sysdt, int minutes)
        {
            DateTime dt = DataType.ParseSysDate2DateTime(sysdt);
            return AddMinutes(dt, 0, minutes);
        }
        /// <summary>
        /// 在指定的日期上增加n天n小时，并考虑节假日
        /// </summary>
        /// <param name="sysdt">指定的日期</param>
        /// <param name="day">天数</param>
        /// <param name="minutes">分钟数</param>
        /// <returns>返回计算后的日期</returns>
        public static DateTime AddDayHoursSpan(string specDT, int day, int hh, int minutes, TWay tway)
        {
            DateTime mydt = DataType.AddDays(specDT, day, tway);
            return Glo.AddMinutes(mydt, hh, minutes);
        }
        /// <summary>
        /// 在指定的日期上增加n天n小时，并考虑节假日
        /// </summary>
        /// <param name="sysdt">指定的日期</param>
        /// <param name="day">天数</param>
        /// <param name="minutes">分钟数</param>
        /// <returns>返回计算后的日期</returns>
        public static DateTime AddDayHoursSpan(DateTime specDT, int day, int hh, int minutes, TWay tway)
        {
            DateTime mydt = DataType.AddDays(specDT, day, tway);
            mydt = mydt.AddHours(hh); //加小时.
            mydt = mydt.AddMinutes(minutes); //加分钟.
            return mydt;
            //return Glo.AddMinutes(mydt, minutes);
        }
        #endregion ssxxx.

        #region 与考核相关.
        /// <summary>
        /// 当流程发送下去以后，就开始执行考核。
        /// </summary>
        /// <param name="fl"></param>
        /// <param name="nd"></param>
        /// <param name="workid"></param>
        /// <param name="fid"></param>
        /// <param name="title"></param>
        public static void InitCH(Flow fl, Node nd, Int64 workid, Int64 fid, string title, GenerWorkerList gwl = null)
        {
            InitCH2017(fl, nd, workid, fid, title, null, null, DateTime.Now, gwl);
        }
        /// <summary>
        /// 执行考核
        /// </summary>
        /// <param name="fl">流程</param>
        /// <param name="nd">节点</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">FID</param>
        /// <param name="title">标题</param>
        /// <param name="prvRDT">上一个时间点</param>
        /// <param name="sdt">应完成日期</param>
        /// <param name="dtNow">当前日期</param>
        private static void InitCH2017(Flow fl, Node nd, Int64 workid, Int64 fid, string title, string prvRDT, string sdt,
            DateTime dtNow, GenerWorkerList gwl)
        {

            // 开始节点不考核.
            if (nd.ItIsStartNode || nd.HisCHWay == CHWay.None)
                return;

            //如果设置为0,则不考核.
            if (nd.TimeLimit == 0 && nd.TimeLimitHH == 0 && nd.TimeLimitMM == 0)
                return;

            if (dtNow == null)
                dtNow = DateTime.Now;

            #region 求参与人员 todoEmps ，应完成日期 sdt ，与工作派发日期 prvRDT.
            //参与人员.
            string todoEmps = "";
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            if (nd.ItIsEndNode == true && gwl == null)
            {
                /* 如果是最后一个节点，可以使用这样的方式来求人员信息 , */

                #region 求应完成日期，与参与的人集合.
                Paras ps = new Paras();
                switch (BP.Difference.SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                        ps.SQL = "SELECT TOP 1 SDTOfNode, TodoEmps FROM WF_GenerWorkFlow  WHERE WorkID=" + dbstr + "WorkID ";
                        break;
                    case DBType.Oracle:
                    case DBType.KingBaseR3:
                    case DBType.KingBaseR6:
                        ps.SQL = "SELECT SDTOfNode, TodoEmps FROM WF_GenerWorkFlow  WHERE WorkID=" + dbstr + "WorkID  ";
                        break;
                    case DBType.MySQL:
                        ps.SQL = "SELECT SDTOfNode, TodoEmps FROM WF_GenerWorkFlow  WHERE WorkID=" + dbstr + "WorkID  ";
                        break;
                    case DBType.PostgreSQL:
                    case DBType.UX:
                    case DBType.HGDB:
                        ps.SQL = "SELECT SDTOfNode, TodoEmps FROM WF_GenerWorkFlow  WHERE WorkID=" + dbstr + "WorkID  ";
                        break;
                    default:
                        throw new Exception("err@没有判断的数据库类型.");
                }

                ps.Add("WorkID", workid);
                DataTable dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count == 0)
                    return;
                sdt = dt.Rows[0]["SDTOfNode"].ToString(); //应完成日期.
                todoEmps = dt.Rows[0]["TodoEmps"].ToString(); //参与人员.
                #endregion 求应完成日期，与参与的人集合.

                #region 求上一个节点的日期.
                dt = Dev2Interface.Flow_GetPreviousNodeTrack(workid, nd.NodeID);
                if (dt.Rows.Count == 0)
                    return;
                //上一个节点的活动日期.
                prvRDT = dt.Rows[0]["RDT"].ToString();
                #endregion
            }


            if (nd.ItIsEndNode == false)
            {
                if (gwl == null)
                {
                    gwl = new GenerWorkerList();
                    gwl.Retrieve(GenerWorkerListAttr.WorkID, workid,
                        GenerWorkerListAttr.FK_Node, nd.NodeID,
                        GenerWorkerListAttr.FK_Emp, WebUser.No);
                }

                prvRDT = gwl.RDT; // dt.Rows[0]["RDT"].ToString(); //上一个时间点的记录日期.
                sdt = gwl.SDT; //  dt.Rows[0]["SDT"].ToString(); //应完成日期.
                todoEmps = WebUser.No + "," + WebUser.Name + ";";
            }
            #endregion 求参与人员，应完成日期，与工作派发日期.

            #region 求 preSender上一个发送人，preSenderText 发送人姓名
            string preSender = "";
            string preSenderText = "";
            DataTable dt_Sender = Dev2Interface.Flow_GetPreviousNodeTrack(workid, nd.NodeID);
            if (dt_Sender.Rows.Count > 0)
            {
                preSender = dt_Sender.Rows[0]["EmpFrom"].ToString();
                preSenderText = dt_Sender.Rows[0]["EmpFromT"].ToString();
            }
            #endregion

            #region 初始化基础数据.
            BP.WF.Data.CH ch = new CH();
            ch.WorkID = workid;
            ch.FID = fid;
            ch.Title = title;

            //记录当时设定的值.
            ch.TimeLimit = nd.TimeLimit;

            ch.NY = dtNow.ToString("yyyy-MM");

            ch.DTFrom = prvRDT;  //任务下达时间.
            ch.DTTo = dtNow.ToString("yyyy-MM-dd HH:mm:ss"); //时间到.

            ch.SDT = sdt; //应该完成时间.

            ch.FlowNo = nd.FlowNo; //流程信息.
            ch.FlowT = nd.FlowName;

            ch.NodeID = nd.NodeID; //节点.
            ch.NodeT = nd.Name;

            ch.DeptNo = WebUser.DeptNo; //部门.
            ch.DeptT = WebUser.DeptName;

            ch.EmpNo = WebUser.No;//当事人.
            ch.EmpT = WebUser.Name;

            // 处理相关联的当事人.
            ch.GroupEmpsNames = todoEmps;
            //上一步发送人
            ch.Sender = preSender;
            ch.SenderT = preSenderText;
            //考核状态
            ch.DTSWay = (int)nd.HisCHWay;

            //求参与人员数量.
            string[] strs = todoEmps.Split(';');
            ch.GroupEmpsNum = strs.Length - 1; //个数.

            //求参与人的ids.
            string empids = ",";
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;

                string[] mystr = str.Split(',');
                empids += mystr[0] + ",";
            }
            ch.GroupEmps = empids;

            // mypk.
            ch.setMyPK(nd.NodeID + "_" + workid + "_" + fid + "_" + WebUser.No);
            #endregion 初始化基础数据.

            #region 求计算属性.
            //求出是第几个周.
            System.Globalization.CultureInfo myCI =
                        new System.Globalization.CultureInfo("zh-CN");
            ch.WeekNum = myCI.Calendar.GetWeekOfYear(dtNow, System.Globalization.CalendarWeekRule.FirstDay, System.DayOfWeek.Monday);

            // UseDays . 求出实际使用天数.
            DateTime dtFrom = DataType.ParseSysDate2DateTime(ch.DTFrom);
            DateTime dtTo = DataType.ParseSysDate2DateTime(ch.DTTo);

            TimeSpan ts = dtTo - dtFrom;
            ch.UseDays = ts.Days;//用时，天数
            ch.UseMinutes = ts.Minutes;//用时，分钟
            //int hour = ts.Hours;
            //ch.UseDays += ts.Hours / 8; //使用的天数.
            if (DataType.IsNullOrEmpty(ch.SDT) == false && ch.SDT.Equals("无") == false)
            {
                // OverDays . 求出 逾期天 数.
                DateTime sdtOfDT = DataType.ParseSysDate2DateTime(ch.SDT);

                TimeSpan myts = dtTo - sdtOfDT;
                ch.OverDays = myts.Days; //逾期的天数.
                ch.OverMinutes = myts.Minutes;//逾期的分钟数
                if (sdtOfDT >= dtTo)
                {
                    /* 正常完成 */
                    ch.CHSta = CHSta.AnQi; //按期完成.
                    ch.Points = 0;
                }
                else
                {
                    /*逾期完成.*/
                    ch.CHSta = CHSta.YuQi; //逾期完成.
                    ch.Points = float.Parse((ch.OverDays * nd.TCent).ToString("0.00"));
                }
            }
            else
            {
                /* 正常完成 */
                ch.CHSta = CHSta.AnQi; //按期完成.
                ch.Points = 0;
            }

            #endregion 求计算属性.
            if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                ch.SetValByKey(CHAttr.OrgNo, WebUser.OrgNo);
            //执行保存.
            try
            {
                ch.DirectInsert();
            }
            catch
            {
                if (ch.IsExits == true)
                {
                    ch.Update();
                }
                else
                {
                    //如果遇到退回的情况就可能涉及到主键重复的问题.
                    ch.setMyPK(DBAccess.GenerGUID());
                    ch.Insert();
                }
            }
        }
        /// <summary>
        /// 中午时间从
        /// </summary>
        public static string AMFrom
        {
            get
            {
                return BP.Difference.SystemConfig.GetValByKey("AMFrom", "08:30");
            }
        }
        /// <summary>
        /// 中午时间从
        /// </summary>
        public static int AMFromInt
        {
            get
            {
                return int.Parse(Glo.AMFrom.Replace(":", ""));
            }
        }
        /// <summary>
        /// 一天有效的工作小时数
        /// 是中午工作小时+下午工作小时.
        /// </summary>
        public static float AMPMHours
        {
            get
            {
                return BP.Difference.SystemConfig.GetValByKeyFloat("AMPMHours", 8);
            }
        }
        /// <summary>
        /// 中午间隔的小时数
        /// </summary>
        public static float AMPMTimeSpan
        {
            get
            {
                return BP.Difference.SystemConfig.GetValByKeyFloat("AMPMTimeSpan", 1);
            }
        }
        /// <summary>
        /// 中午时间到
        /// </summary>
        public static string AMTo
        {
            get
            {
                return BP.Difference.SystemConfig.GetValByKey("AMTo", "11:30");
            }
        }
        /// <summary>
        /// 中午时间到
        /// </summary>
        public static int AMToInt
        {
            get
            {
                return int.Parse(Glo.AMTo.Replace(":", ""));
            }
        }
        /// <summary>
        /// 下午时间从
        /// </summary>
        public static string PMFrom
        {
            get
            {
                return BP.Difference.SystemConfig.GetValByKey("PMFrom", "13:30");
            }
        }
        /// <summary>
        /// 到
        /// </summary>
        public static int PMFromInt
        {
            get
            {
                return int.Parse(Glo.PMFrom.Replace(":", ""));
            }
        }
        /// <summary>
        /// 到
        /// </summary>
        public static string PMTo
        {
            get
            {
                return BP.Difference.SystemConfig.GetValByKey("PMTo", "17:30");
            }
        }
        /// <summary>
        /// 到
        /// </summary>
        public static int PMToInt
        {
            get
            {
                return int.Parse(Glo.PMTo.Replace(":", ""));
            }
        }
        #endregion 与考核相关.

        #region 其他方法。

        /// <summary>
        /// 删除临时文件
        /// </summary>
        public static void DeleteTempFiles()
        {
            try
            {
                //删除目录.
                string temp = BP.Difference.SystemConfig.PathOfTemp;
                System.IO.Directory.Delete(temp, true);

                //创建目录.
                System.IO.Directory.CreateDirectory(temp);

                //删除pdf 目录.
                temp = BP.Difference.SystemConfig.PathOfDataUser + "InstancePacketOfData/";
                System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(temp);
                System.IO.DirectoryInfo[] dirs = info.GetDirectories();
                foreach (System.IO.DirectoryInfo dir in dirs)
                {
                    if (dir.Name.IndexOf("ND") == 0)
                        dir.Delete(true);
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 复制表单权限-从一个节点到另一个节点.
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="frmID">表单ID</param>
        /// <param name="currNodeID">当前节点</param>
        /// <param name="fromNodeID">从节点</param>
        public static void CopyFrmSlnFromNodeToNode(string fk_flow, string frmID, int currNodeID, int fromNodeID)
        {
            #region 处理字段.
            //删除现有的.
            FrmFields frms = new FrmFields();
            frms.Delete(FrmFieldAttr.FK_Node, currNodeID, FrmFieldAttr.FrmID, frmID);

            //查询出来,指定的权限方案.
            frms.Retrieve(FrmFieldAttr.FK_Node, fromNodeID, FrmFieldAttr.FrmID, frmID);

            //开始复制.
            foreach (FrmField item in frms)
            {
                item.setMyPK(frmID + "_" + fk_flow + "_" + currNodeID + "_" + item.KeyOfEn);
                item.NodeID = currNodeID;
                item.Insert(); // 插入数据库.
            }
            #endregion 处理字段.

            //没有考虑到附件的权限 20161020 hzm
            #region 附件权限

            FrmAttachments fas = new FrmAttachments();
            //删除现有节点的附件权限
            fas.Delete(FrmAttachmentAttr.FK_Node, currNodeID, FrmAttachmentAttr.FK_MapData, frmID);
            //查询出 现在表单上是否有附件的情况
            fas.Retrieve(FrmAttachmentAttr.FK_Node, fromNodeID, FrmAttachmentAttr.FK_MapData, frmID);

            //复制权限
            foreach (FrmAttachment fa in fas)
            {
                fa.setMyPK(fa.FrmID + "_" + fa.NoOfObj + "_" + currNodeID);
                fa.NodeID = currNodeID;
                fa.Insert();
            }

            #endregion

        }

        private const string StrRegex = @"-|;|,|/|(|)|[|]|}|{|%|@|*|!|'|`|~|#|$|^|&|.|?";
        private const string StrKeyWord = @"select|insert|delete|from|count(|drop table|update|truncate|asc(|mid(|char(|xp_cmdshell|exec master|netlocalgroup administrators|:|net user|""|or|and";
        /// <summary>
        /// 检查KeyWord是否包涵特殊字符
        /// </summary>
        /// <param name="_sWord">需要检查的字符串</param>
        /// <returns></returns>
        public static string CheckKeyWord(string KeyWord)
        {
            //特殊符号
            string[] strRegx = StrRegex.Split('|');
            //特殊符号 的注入情况
            foreach (string key in strRegx)
            {
                if (KeyWord.IndexOf(key) >= 0)
                {
                    //替换掉特殊字符
                    KeyWord = KeyWord.Replace(key, "");
                }
            }
            return KeyWord;
        }
        /// <summary>
        /// 检查_sword是否包涵SQL关键字
        /// </summary>
        /// <param name="_sWord">需要检查的字符串</param>
        /// <returns>存在SQL注入关键字时返回 true，否则返回 false</returns>
        public static bool CheckKeyWordInSql(string _sWord)
        {
            bool result = false;
            //Sql注入de可能关键字
            string[] patten1 = StrKeyWord.Split('|');
            //Sql注入的可能关键字 的注入情况
            foreach (string sqlKey in patten1)
            {
                if (_sWord.IndexOf(" " + sqlKey) >= 0 || _sWord.IndexOf(sqlKey + " ") >= 0)
                {
                    //只要存在一个可能出现Sql注入的参数,则直接退出
                    result = true;
                    break;
                }
            }
            return result;
        }
        #endregion 其他方法。
    }
}
