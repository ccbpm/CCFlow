using System;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using Microsoft.Win32;
using BP.Sys;
using BP.DA;
using BP.En;
using BP;
using BP.Web;
using System.Security.Cryptography;
using System.Text;
using BP.Port;
using BP.WF.Rpt;
using BP.WF.Data;
using BP.WF.Template;

namespace BP.WF
{
    /// <summary>
    /// 全局(方法处理)
    /// </summary>
    public class Glo
    {
        #region 公共属性.
        /// <summary>
        /// 运行平台.
        /// </summary>
        public static Platform Platform
        {
            get
            {
                return  Platform.CCFlow;
            }
        }
        /// <summary>
        /// 得到WebService对象 
        /// </summary>
        /// <returns></returns>
        public static CCInterface.PortalInterfaceSoapClient GetPortalInterfaceSoapClient()
        {
            TimeSpan ts = new TimeSpan(0, 5, 0);
            var basicBinding = new BasicHttpBinding()
            {
                ReceiveTimeout = ts,
                SendTimeout = ts,
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,
                Name = "PortalInterfaceSoap"
            };
            basicBinding.Security.Mode = BasicHttpSecurityMode.None;

            string url = "";
            if (Glo.Platform ==  Platform.CCFlow)
            {
                url = "/DataUser/PortalInterface.asmx";
                url = Glo.HostURL + url;
            }
            else
            {
                //  url = string.Format("/{0}webservices/webservice.*", AppName != string.Empty ? AppName + "/" : string.Empty);
            //    url = new Uri(App.Current.Host.Source, "../").ToString() + "service/Service?wsdl";

            }

            url = url.Replace("//", "/");
            url = url.Replace(":/", "://");

            //  MessageBox.Show(url);

            var endPoint = new EndpointAddress(url);
            var ctor =
                typeof(CCInterface.PortalInterfaceSoapClient).GetConstructor(
                new Type[] {
                    typeof(Binding), 
                    typeof(EndpointAddress)
                });
            return (CCInterface.PortalInterfaceSoapClient)ctor.Invoke(
                new object[] { basicBinding, endPoint });
        }
        /// <summary>
        /// 短消息写入类型
        /// </summary>
        public static ShortMessageWriteTo ShortMessageWriteTo
        {
            get
            {
                return (ShortMessageWriteTo)BP.Sys.SystemConfig.GetValByKeyInt("ShortMessageWriteTo", 0);
            }
        }
        /// <summary>
        /// 当前选择的流程.
        /// </summary>
        public static string CurrFlow
        {
            get
            {
                return System.Web.HttpContext.Current.Session["CurrFlow"] as string;
            }
            set
            {
                System.Web.HttpContext.Current.Session["CurrFlow"] = value;
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

        #region 执行安装/升级.
        /// <summary>
        /// 执行升级
        /// </summary>
        /// <returns></returns>
        public static string UpdataCCFlowVer()
        {
            #region 检查是否需要升级，并更新升级的业务逻辑.
            string val = "20160526";
            string updataNote = "";
            updataNote += "20160526.升级FrmEnableRole状态.";
            updataNote += "20160501.升级todosta状态.";
            updataNote += "20160420.升级表单属性.";
            updataNote += "20160226.新版流程FlowJson字段判断..";
            updataNote += "20151230.升级阻塞规则..";
            updataNote += "20151208.为国机升级找人规则.";
            updataNote += "20151205.为流程增加一个周.";
            updataNote += "20151115.升级自由流程规则.";
            updataNote += "20151008.升级节点访问规则描述";
            updataNote += "20151005.升级表单树，dbsrc..";
            updataNote += "20151004.升级表单，apptype..";
            updataNote += "20150922.升级表单..";
            updataNote += "20150902.升级父子流程..";
            updataNote += "20150814.升级新昌的公文..";
            updataNote += "20150730.增加考核表.";
            updataNote += "20150725.升级流程查看权限规则.";
            updataNote += "20150613.升级CCRole.";
            updataNote += "201505302.升级FWCType.";
            updataNote += "2015051673.升级表单属性.";
            updataNote += "20150516. 为流程引擎增加了数据同步功能.";
            updataNote += "20150508. 增加删除bpm模式的一个视图 by:DaiGuoQiang.";
            updataNote += "20150506. 增加了bpm模式的一个视图.";
            updataNote += "20150505. 处理了岗位枚举值的问题 by:zhoupeng.";
            updataNote += "20150424. 优化发起列表的效率, by:zhoupeng.";

            /*
             * 升级版本记录:
             * 20150330: 优化发起列表的效率, by:zhoupeng.
             * 2, 升级表单树,支持动态表单树.
             * 1, 执行一次Sender发送人的升级，原来由GenerWorkerList 转入WF_GenerWorkFlow.
             * 0, 静默升级启用日期.2014-12
             */
            string sql = "SELECT IntVal FROM Sys_Serial WHERE CfgKey='Ver'";
            string currVer = DBAccess.RunSQLReturnStringIsNull(sql, "");
            if (currVer == val)
                return null; //不需要升级.
            #endregion 检查是否需要升级，并更新升级的业务逻辑.

            string msg = "";
            try
            {
                //执行升级 2016.04.08 
                BP.WF.Template.Cond cnd = new Cond();
                cnd.CheckPhysicsTable();

                #region 标签Ext
                sql = "DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.NodeExt'";
                BP.DA.DBAccess.RunSQL(sql);
                sql = "INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.NodeExt','";
                sql += "@NodeID=基本配置";
                sql += "@FWCSta=审核组件,适用于sdk表单审核组件与ccform上的审核组件属性设置.";
                sql += "@SendLab=按钮权限,控制工作节点可操作按钮.";
                sql += "@RunModel=运行模式,分合流,父子流程";
                sql += "@AutoJumpRole0=跳转,自动跳转规则当遇到该节点时如何让其自动的执行下一步.";
                sql += "@MPhone_WorkModel=移动,与手机平板电脑相关的应用设置.";
                sql += "@TSpanDay=考核,时效考核,质量考核.";
             //   sql += "@OfficeOpen=公文按钮,只有当该节点是公文流程时候有效";
                sql += "')";
                BP.DA.DBAccess.RunSQL(sql);

                sql = "DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.FlowExt'";
                BP.DA.DBAccess.RunSQL(sql);
                sql = "INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.FlowExt','";
                sql += "@No=基本配置";
                sql += "')";
                BP.DA.DBAccess.RunSQL(sql);

                sql = "DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.MapDataExt'";
                BP.DA.DBAccess.RunSQL(sql);
                sql = "INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.MapDataExt','";
                sql += "@No=基本属性";
                sql += "@Designer=设计者信息";
                sql += "')";
                BP.DA.DBAccess.RunSQL(sql);

                //更新表单应用类型，注意会涉及到其他问题.
                sql = "UPDATE Sys_MapData SET AppType=0 WHERE No NOT LIKE 'ND%'";
                DBAccess.RunSQL(sql);
                #endregion

                #region 标签
                sql = "DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.NodeSheet'";
                BP.DA.DBAccess.RunSQL(sql);
                sql = "INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.NodeSheet','";
                sql += "@NodeID=基本配置";
                sql += "@FormType=表单";
                sql += "@FWCSta=审核组件,适用于sdk表单审核组件与ccform上的审核组件属性设置.";
                sql += "@SFSta=父子流程,对启动，查看父子流程的控件设置.";
                sql += "@SendLab=按钮权限,控制工作节点可操作按钮.";
                sql += "@RunModel=运行模式,分合流,父子流程";
                sql += "@AutoJumpRole0=跳转,自动跳转规则当遇到该节点时如何让其自动的执行下一步.";
                sql += "@MPhone_WorkModel=移动,与手机平板电脑相关的应用设置.";
                sql += "@TSpanDay=考核,时效考核,质量考核.";
                //  sql += "@MsgCtrl=消息,流程消息信息.";
                sql += "@OfficeOpen=公文按钮,只有当该节点是公文流程时候有效";
                sql += "')";
                BP.DA.DBAccess.RunSQL(sql);

                sql = "DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.FlowSheet'";
                BP.DA.DBAccess.RunSQL(sql);
                sql = "INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.FlowSheet','";
                sql += "@No=基本配置";
                sql += "@FlowRunWay=启动方式,配置工作流程如何自动发起，该选项要与流程服务一起工作才有效.";
                sql += "@StartLimitRole=启动限制规则";
                sql += "@StartGuideWay=发起前置导航";
                sql += "@CFlowWay=延续流程";
                sql += "@DTSWay=流程数据与业务数据同步";
                sql += "@PStarter=轨迹查看权限";
                sql += "')";
                BP.DA.DBAccess.RunSQL(sql);
                #endregion

                #region  增加week字段,方便按周统计.
                BP.WF.GenerWorkFlow gwf = new GenerWorkFlow();
                gwf.CheckPhysicsTable();
                sql = "SELECT WorkID,RDT FROM WF_GenerWorkFlow WHERE WeekNum=0 or WeekNum is null ";
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    sql = "UPDATE WF_GenerWorkFlow SET WeekNum=" + BP.DA.DataType.CurrentWeekGetWeekByDay(dr[1].ToString()) + " WHERE WorkID="+dr[0].ToString();
                    BP.DA.DBAccess.RunSQL(sql);
                }
                //查询.
                BP.WF.Data.CH ch = new CH();
                ch.CheckPhysicsTable();

                sql = "SELECT MyPK,DTFrom FROM WF_CH WHERE WeekNum=0 or WeekNum is null ";
                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    sql = "UPDATE WF_CH SET Week=" + BP.DA.DataType.CurrentWeekGetWeekByDay(dr[1].ToString()) + " WHERE MyPK='" + dr[0].ToString()+"'";
                    BP.DA.DBAccess.RunSQL(sql);
                }
                #endregion  增加week字段.

                #region 检查数据源.
                SFDBSrc src = new SFDBSrc();
                src.No = "local";
                src.Name = "本机数据源(默认)";
                if (src.RetrieveFromDBSources()==0)
                    src.Insert();
                #endregion 检查数据源.

                #region 更新枚举类型.
                //删除枚举值,让其自动生成.
                sql = "DELETE FROM Sys_Enum WHERE EnumKey IN ('CodeStruct'";
                sql += ",'DBSrcType'";
                sql += ",'WebOfficeEnable'";
                sql += ",'FrmEnableRole'";
                sql += ",'BlockModel'";
                sql += ",'CCRole','FWCType','SelectAccepterEnable','NodeFormType','StartGuideWay','" + FlowAttr.StartLimitRole + "','BillFileType','EventDoType','FormType','BatchRole','StartGuideWay','NodeFormType','FormRunType')";
                BP.DA.DBAccess.RunSQL(sql);
                #endregion 更新枚举类型.

                #region 其他.
                // 更新 PassRate.
                sql = "UPDATE WF_Node SET PassRate=100 WHERE PassRate IS NULL";
                BP.DA.DBAccess.RunSQL(sql);
                #endregion 其他.

                #region 升级统一规则.
                try
                {
                    string sqls = "";
                    sqls += "UPDATE Sys_MapExt SET MyPK= ExtType+'_'+FK_Mapdata+'_'+AttrOfOper WHERE ExtType='" + MapExtXmlList.TBFullCtrl + "'";
                    sqls += "UPDATE Sys_MapExt SET MyPK= ExtType+'_'+FK_Mapdata+'_'+AttrOfOper WHERE ExtType='" + MapExtXmlList.PopVal + "'";
                    sqls += "UPDATE Sys_MapExt SET MyPK= ExtType+'_'+FK_Mapdata+'_'+AttrOfOper WHERE ExtType='" + MapExtXmlList.DDLFullCtrl + "'";
                    sqls += "UPDATE Sys_MapExt SET MyPK= ExtType+'_'+FK_Mapdata+'_'+AttrOfOper WHERE ExtType='" + MapExtXmlList.ActiveDDL + "'";
                    BP.DA.DBAccess.RunSQLs(sqls);
                }
                catch
                {
                }
                #endregion


                //#region  更新CA签名(2015-03-03)。
                //BP.Tools.WFSealData sealData = new Tools.WFSealData();
                //sealData.CheckPhysicsTable();
                //sealData.UpdateColumn();
                //#endregion  更新CA签名.

                #region 其他.
                //升级表单树. 2015.10.05
                SysFormTree sft = new SysFormTree();
                sft.CheckPhysicsTable();
                sql = "UPDATE Sys_FormTree SET DBSrc='local'  WHERE DBSrc IS NULL OR DBSrc=''";
                BP.DA.DBAccess.RunSQL(sql);
             

                //表单信息表.
                MapDataExt mapext = new MapDataExt();
                mapext.CheckPhysicsTable();
                
                TransferCustom tc = new TransferCustom();
                tc.CheckPhysicsTable();

                //增加部门字段。
                CCList cc = new CCList();
                cc.CheckPhysicsTable();
                #endregion 其他.

                #region 判断WF_Flow中是否含有FlowJson字段，没有则增加此字段，edited by liuxc,2016-2-25

                DataTable columns = src.GetColumns("WF_Flow");
                if (columns.Select("No='FlowJson'").Length == 0)
                {
                    switch(src.HisDBType)
                    {
                        case DBType.MSSQL:
                            DBAccess.RunSQL("ALTER TABLE WF_Flow ADD FlowJson IMAGE NULL");
                            break;
                        case DBType.Oracle:
                        case DBType.Informix:
                            DBAccess.RunSQL("ALTER TABLE WF_Flow ADD FlowJson CLOB NULL");
                            break;
                        case DBType.MySQL:
                            DBAccess.RunSQL("ALTER TABLE WF_Flow ADD FlowJson TEXT");
                            break;
                        default:
                            break;
                    }
                }
                #endregion

                #region 升级sys_sftable
                //升级
                BP.Sys.SFTable tab = new SFTable();
                tab.CheckPhysicsTable();
                //mySQL = "UPDATE Sys_SFTable SET "+SFTableAttr.SrcTable+"=0 WHERE TabType IS NULL";
                //BP.DA.DBAccess.RunSQL(mySQL);
                #endregion

                #region 基础数据更新.
                Node wf_Node = new Node();
                wf_Node.CheckPhysicsTable();
                // 设置节点ICON.
                sql = "UPDATE WF_Node SET ICON='/WF/Data/NodeIcon/审核.png' WHERE ICON IS NULL";
                BP.DA.DBAccess.RunSQL(sql);

                BP.WF.Template.NodeSheet nodeSheet = new BP.WF.Template.NodeSheet();
                nodeSheet.CheckPhysicsTable();
                // 升级手机应用. 2014-08-02.
                sql = "UPDATE WF_Node SET MPhone_WorkModel=0,MPhone_SrcModel=0,MPad_WorkModel=0,MPad_SrcModel=0 WHERE MPhone_WorkModel IS NULL";
                BP.DA.DBAccess.RunSQL(sql);
                #endregion 基础数据更新.

                #region 把节点的toolbarExcel, word 信息放入mapdata
                BP.WF.Template.NodeSheets nss = new Template.NodeSheets();
                nss.RetrieveAll();
                foreach (BP.WF.Template.NodeSheet ns in nss)
                {
                    ToolbarExcel te = new ToolbarExcel();
                    te.No = "ND" + ns.NodeID;
                    te.RetrieveFromDBSources();

                    //te.Copy(ns);
                    //te.Update();
                }
                #endregion

                #region 升级SelectAccper
                Direction dir = new Direction();
                dir.CheckPhysicsTable();

                SelectAccper selectAccper = new SelectAccper();
                selectAccper.CheckPhysicsTable();
                #endregion

                #region 升级wf-generworkerlist 2014-05-09
                GenerWorkerList gwl = new GenerWorkerList();
                gwl.CheckPhysicsTable();
                #endregion 升级wf-generworkerlist 2014-05-09

                #region  升级 NodeToolbar
                FrmField ff = new FrmField();
                ff.CheckPhysicsTable();

                MapAttr attr = new MapAttr();
                attr.CheckPhysicsTable();

                NodeToolbar bar = new NodeToolbar();
                bar.CheckPhysicsTable();

                BP.WF.Template.FlowFormTree tree = new BP.WF.Template.FlowFormTree();
                tree.CheckPhysicsTable();

                FrmNode nff = new FrmNode();
                nff.CheckPhysicsTable();

                SysForm ssf = new SysForm();
                ssf.CheckPhysicsTable();

                SysFormTree ssft = new SysFormTree();
                ssft.CheckPhysicsTable();

                BP.Sys.FrmAttachment ath = new FrmAttachment();
                ath.CheckPhysicsTable();

                FrmField ffs = new FrmField();
                ffs.CheckPhysicsTable();
                #endregion

                #region 执行sql．
                BP.DA.DBAccess.RunSQL("delete  from Sys_Enum WHERE EnumKey in ('BillFileType','EventDoType','FormType','BatchRole','StartGuideWay','NodeFormType')");
                DBAccess.RunSQL("UPDATE Sys_FrmSln SET FK_Flow =(SELECT FK_FLOW FROM WF_Node WHERE NODEID=Sys_FrmSln.FK_Node) WHERE FK_Flow IS NULL");
               
                try
                {
                    DBAccess.RunSQL("UPDATE WF_FrmNode SET MyPK=FK_Frm+'_'+convert(varchar,FK_Node )+'_'+FK_Flow");
                }
                catch
                {

                }
                #endregion

                #region 检查必要的升级。
                //部门
                BP.Port.Dept d = new BP.Port.Dept();
                d.CheckPhysicsTable();

                FrmWorkCheck fwc = new FrmWorkCheck();
                fwc.CheckPhysicsTable();


                Flow myfl = new Flow();
                myfl.CheckPhysicsTable();

                Node nd = new Node();
                nd.CheckPhysicsTable();
                //Sys_SFDBSrc
                SFDBSrc sfDBSrc = new SFDBSrc();
                sfDBSrc.CheckPhysicsTable();
                #endregion 检查必要的升级。

                #region 执行更新.wf_node
                sql = "UPDATE WF_Node SET FWCType=0 WHERE FWCType IS NULL";
                sql += "@UPDATE WF_Node SET FWC_X=0 WHERE FWC_X IS NULL";
                sql += "@UPDATE WF_Node SET FWC_Y=0 WHERE FWC_Y IS NULL";
                sql += "@UPDATE WF_Node SET FWC_W=0 WHERE FWC_W IS NULL";
                sql += "@UPDATE WF_Node SET FWC_H=0 WHERE FWC_H IS NULL";
                BP.DA.DBAccess.RunSQLs(sql);


                sql = "UPDATE WF_Node SET SFSta=0 WHERE SFSta IS NULL";
                sql += "@UPDATE WF_Node SET SF_X=0 WHERE SF_X IS NULL";
                sql += "@UPDATE WF_Node SET SF_Y=0 WHERE SF_Y IS NULL";
                sql += "@UPDATE WF_Node SET SF_W=0 WHERE SF_W IS NULL";
                sql += "@UPDATE WF_Node SET SF_H=0 WHERE SF_H IS NULL";
                BP.DA.DBAccess.RunSQLs(sql);

                #endregion 执行更新.

                #region 执行报表设计。
                Flows fls = new Flows();
                fls.RetrieveAll();
                foreach (Flow fl in fls)
                {
                    try
                    {
                        MapRpts rpts = new MapRpts(fl.No);
                    }
                    catch
                    {
                        fl.DoCheck();
                    }
                }
                #endregion

                #region 升级站内消息表 2013-10-20
                BP.WF.SMS sms = new SMS();
                sms.CheckPhysicsTable();
                #endregion

                #region 重新生成view WF_EmpWorks,  2013-08-06.
                try
                {
                    BP.DA.DBAccess.RunSQL("DROP VIEW WF_EmpWorks");
                    BP.DA.DBAccess.RunSQL("DROP VIEW V_FlowStarter");
                    BP.DA.DBAccess.RunSQL("DROP VIEW V_FlowStarterBPM");
                    BP.DA.DBAccess.RunSQL("DROP VIEW V_TOTALCH");
                    BP.DA.DBAccess.RunSQL("DROP VIEW V_TOTALCHYF");
                    BP.DA.DBAccess.RunSQL("DROP VIEW V_TOTALCHWeek");
                }
                catch
                {
                }


                string sqlscript = "";
                //执行必须的sql.
                switch (BP.Sys.SystemConfig.AppCenterDBType)
                {
                    case DBType.Oracle:
                        sqlscript = BP.Sys.SystemConfig.CCFlowAppPath + "\\WF\\Data\\Install\\SQLScript\\InitView_Ora.sql";
                        break;
                    case DBType.MSSQL:
                    case DBType.Informix:
                        sqlscript = BP.Sys.SystemConfig.CCFlowAppPath + "\\WF\\Data\\Install\\SQLScript\\InitView_SQL.sql";
                        break;
                    case DBType.MySQL:
                        sqlscript = BP.Sys.SystemConfig.CCFlowAppPath + "\\WF\\Data\\Install\\SQLScript\\InitView_MySQL.sql";
                        break;
                    default:
                        break;
                }

                BP.DA.DBAccess.RunSQLScript(sqlscript);
                #endregion

                #region 更新表单的边界.2014-10-18
                MapDatas mds = new MapDatas();
                mds.RetrieveAll();

                foreach (MapData md in mds)
                    md.ResetMaxMinXY(); //更新边界.
                #endregion 更新表单的边界.

                #region 升级Img
                FrmImg img = new FrmImg();
                img.CheckPhysicsTable();
                #endregion

                #region 修复 mapattr UIHeight, UIWidth 类型错误.
                switch (BP.Sys.SystemConfig.AppCenterDBType)
                {
                    case DBType.Oracle:
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
                switch (BP.Sys.SystemConfig.AppCenterDBType)
                {
                    case DBType.Oracle:
                        int i = DBAccess.RunSQLReturnCOUNT("SELECT * FROM USER_TAB_COLUMNS WHERE TABLE_NAME = 'SYS_DEFVAL' AND COLUMN_NAME = 'PARENTNO'");
                        if (i == 0)
                        {
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
                DBAccess.RunSQL("DELETE FROM Sys_Enum WHERE EnumKey IN ('DeliveryWay','RunModel','OutTimeDeal','FlowAppType')");
                #endregion 登陆更新错误

                #region 升级表单树
                // 首先检查是否升级过.
                sql = "SELECT * FROM Sys_FormTree WHERE No = '0'";
                DataTable formTree_dt = DBAccess.RunSQLReturnTable(sql);
                if (formTree_dt.Rows.Count == 0)
                {
                    /*没有升级过.增加根节点*/
                    SysFormTree formTree = new SysFormTree();
                    formTree.No = "0";
                    formTree.Name = "表单库";
                    formTree.ParentNo = "";
                   // formTree.TreeNo = "0";
                    formTree.Idx = 0;
                    formTree.IsDir = true;

                    try
                    {
                        formTree.DirectInsert();
                    }
                    catch
                    {
                    }
                    //将表单库中的数据转入表单树
                    SysFormTrees formSorts = new SysFormTrees();
                    formSorts.RetrieveAll();

                    foreach (SysFormTree item in formSorts)
                    {
                        if (item.No == "0")
                            continue;
                        SysFormTree subFormTree = new SysFormTree();
                        subFormTree.No = item.No;
                        subFormTree.Name = item.Name;
                        subFormTree.ParentNo = "0";
                        subFormTree.Save();
                    }
                    //表单于表单树进行关联
                    sql = "UPDATE Sys_MapData SET FK_FormTree=FK_FrmSort WHERE FK_FrmSort <> '' AND FK_FrmSort is not null";
                    DBAccess.RunSQL(sql);
                }
                #endregion

                #region 执行admin登陆. 2012-12-25 新版本.
                Emp emp = new Emp();
                emp.No = "admin";
                if (emp.RetrieveFromDBSources() == 1)
                {
                    BP.Web.WebUser.SignInOfGener(emp, true);
                }
                else
                {
                    emp.No = "admin";
                    emp.Name = "admin";
                    emp.FK_Dept = "01";
                    emp.Pass = "123";
                    emp.Insert();
                    BP.Web.WebUser.SignInOfGener(emp, true);
                    //throw new Exception("admin 用户丢失，请注意大小写。");
                }
                #endregion 执行admin登陆.

                #region 修复 Sys_FrmImg 表字段 ImgAppType Tag0
                switch (BP.Sys.SystemConfig.AppCenterDBType)
                {
                    case DBType.Oracle:
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

                // 最后更新版本号，然后返回.
                sql = "UPDATE Sys_Serial SET IntVal=" + val + " WHERE CfgKey='Ver'";
                if (DBAccess.RunSQL(sql) == 0)
                {
                    sql = "INSERT INTO Sys_Serial (CfgKey,IntVal) VALUES('Ver'," + val + ") ";
                    DBAccess.RunSQL(sql);
                }
                // 返回版本号.
                return val; // +"\t\n解决问题:" + updataNote;
            }
            catch (Exception ex)
            {
                string err= "问题出处:" + ex.Message + "<hr>" + msg + "<br>详细信息:@" + ex.StackTrace + "<br>@<a href='../DBInstall.aspx' >点这里到系统升级界面。</a>";
                BP.DA.Log.DebugWriteError("系统升级错误:"+err);
                return "0";
                //return "升级失败,详细请查看日志.\\DataUser\\Log\\";
            }
        }
        /// <summary>
        /// CCFlowAppPath
        /// </summary>
        public static string CCFlowAppPath
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKey("CCFlowAppPath", "/");
            }
        }
        /// <summary>
        /// 安装包
        /// </summary>
        public static void DoInstallDataBase(string lang, bool isInstallFlowDemo,bool isInstallCCIM)
        {
            ArrayList al = null;
            string info = "BP.En.Entity";
            al = BP.En.ClassFactory.GetObjects(info);

            #region 1, 创建or修复表
            foreach (Object obj in al)
            {
                Entity en = null;
                en = obj as Entity;
                if (en == null)
                    continue;

                //获得类名.
                string clsName = en.ToString();

                //不安装CCIM的表.
                if (clsName!=null && clsName.Contains("BP.CCIM"))
                    continue;

                if (isInstallFlowDemo == false)
                {
                    /*如果不安装demo.*/
                    if (clsName != null)
                    {
                        if (clsName.Contains("BP.CN")
                            || clsName.Contains("BP.Demo"))
                            continue;
                    }
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

                switch (table)
                {
                    case "WF_EmpWorks":
                    case "WF_GenerEmpWorkDtls":
                    case "WF_GenerEmpWorks":
                    case "V_FlowData":
                        continue;
                    case "Sys_Enum":
                        en.CheckPhysicsTable();
                        break;
                    default:
                        en.CheckPhysicsTable();
                        break;
                }
                en.PKVal = "123";
                try
                {
                    en.RetrieveFromDBSources();
                }
                catch (Exception ex)
                {
                    Log.DebugWriteWarning(ex.Message);
                    en.CheckPhysicsTable();
                }
            }


            #region 创建 Port_EmpDept 视图兼容旧版本.
            //创建视图.
            try
            {
                BP.DA.DBAccess.RunSQL("DROP TABLE Port_EmpDept");
            }
            catch
            {
            }
            try
            {
                BP.DA.DBAccess.RunSQL("DROP VIEW Port_EmpDept");
            }
            catch
            {
            }
            if (DBAccess.IsExitsObject("Port_EmpDept") == false)
            {
                string sql = "CREATE VIEW Port_EmpDept as SELECT No as FK_Emp, FK_Dept FROM Port_Emp";
                BP.DA.DBAccess.RunSQL(sql);
            }
            #endregion 创建视图.

            #endregion 修复

            #region 2, 注册枚举类型 SQL
            // 2, 注册枚举类型。
            BP.Sys.XML.EnumInfoXmls xmls = new BP.Sys.XML.EnumInfoXmls();
            xmls.RetrieveAll();
            foreach (BP.Sys.XML.EnumInfoXml xml in xmls)
            {
                BP.Sys.SysEnums ses = new BP.Sys.SysEnums();
                ses.RegIt(xml.Key, xml.Vals);
            }
            #endregion 注册枚举类型

            #region 3, 执行基本的 sql 
            if (isInstallFlowDemo == false)
            {
                SysFormTree frmSort = new SysFormTree();
                frmSort.No = "01";
                frmSort.Name = "表单类别1";
                frmSort.ParentNo = "0";
                frmSort.Insert();
            }

            //删除这个数据, 没有找到，初始化这些数据失败的原因.
            BP.DA.DBAccess.RunSQL("DELETE FROM PORT_DEPTSTATION");

            string sqlscript = "";
            if (Glo.OSModel == BP.Sys.OSModel.OneOne)
            {
                /*如果是OneOne模式*/
                sqlscript = BP.Sys.SystemConfig.CCFlowAppPath + "\\WF\\Data\\Install\\SQLScript\\Port_Inc_CH_WorkFlow.sql";
                BP.DA.DBAccess.RunSQLScript(sqlscript);
            }

            if (Glo.OSModel == BP.Sys.OSModel.OneMore)
            {
                /*如果是OneMore模式*/
                sqlscript = BP.Sys.SystemConfig.CCFlowAppPath + "\\GPM\\SQLScript\\Port_Inc_CH_BPM.sql";
                BP.DA.DBAccess.RunSQLScript(sqlscript);
            }
            #endregion 修复

            #region 4, 创建视图与数据.
            //执行必须的sql.

              sqlscript = "";
            //执行必须的sql.
            switch (BP.Sys.SystemConfig.AppCenterDBType)
            {
                case DBType.Oracle:
                    sqlscript = BP.Sys.SystemConfig.CCFlowAppPath + "\\WF\\Data\\Install\\SQLScript\\InitView_Ora.sql";
                    break;
                case DBType.MSSQL:
                case DBType.Informix:
                    sqlscript = BP.Sys.SystemConfig.CCFlowAppPath + "\\WF\\Data\\Install\\SQLScript\\InitView_SQL.sql";
                    break;
                case DBType.MySQL:
                    sqlscript = BP.Sys.SystemConfig.CCFlowAppPath + "\\WF\\Data\\Install\\SQLScript\\InitView_MySQL.sql";
                    break;
                default:
                    break;
            }

            BP.DA.DBAccess.RunSQLScript(sqlscript);
            #endregion 创建视图与数据

            #region 5, 初始化数据.
            if (isInstallFlowDemo)
            {
                sqlscript = SystemConfig.PathOfData + "\\Install\\SQLScript\\InitPublicData.sql";
                BP.DA.DBAccess.RunSQLScript(sqlscript);
            }
            else
            {
                FlowSort fs = new FlowSort();
                fs.No = "02";
                fs.ParentNo = "99";
                fs.Name = "其他类";
                fs.DirectInsert();
            }
            #endregion 初始化数据

            #region 6, 生成临时的 wf_emp 数据。
            if (isInstallFlowDemo)
            {
                BP.Port.Emps emps = new BP.Port.Emps();
                emps.RetrieveAllFromDBSource();
                int i = 0;
                foreach (BP.Port.Emp emp in emps)
                {
                    i++;
                    BP.WF.Port.WFEmp wfEmp = new BP.WF.Port.WFEmp();
                    wfEmp.Copy(emp);
                    wfEmp.No = emp.No;

                    if (wfEmp.Email.Length == 0)
                        wfEmp.Email = emp.No + "@ccflow.org";

                    if (wfEmp.Tel.Length == 0)
                        wfEmp.Tel = "82374939-6" + i.ToString().PadLeft(2, '0');

                    if (wfEmp.IsExits)
                        wfEmp.Update();
                    else
                        wfEmp.Insert();
                }

                // 生成简历数据.
                int oid = 1000;
                foreach (BP.Port.Emp emp in emps)
                {
                    //for (int myIdx = 0; myIdx < 6; myIdx++)
                    //{
                    //    BP.WF.Demo.Resume re = new Demo.Resume();
                    //    re.NianYue = "200" + myIdx + "年01月";
                    //    re.FK_Emp = emp.No;
                    //    re.GongZuoDanWei = "工作部门-" + myIdx;
                    //    re.ZhengMingRen = "张" + myIdx;
                    //    re.BeiZhu = emp.Name + "同志工作认真.";
                    //    oid++;
                    //    re.InsertAsOID(oid);
                    //}
                }
                // 生成年度月份数据.
                string sqls = "";
                DateTime dtNow = DateTime.Now;
                for (int num = 0; num < 12; num++)
                {
                    sqls = "@ INSERT INTO Pub_NY (No,Name) VALUES ('" + dtNow.ToString("yyyy-MM") + "','" + dtNow.ToString("yyyy-MM") + "')  ";
                    dtNow = dtNow.AddMonths(1);
                }
                BP.DA.DBAccess.RunSQLs(sqls);
            }
            #endregion 初始化数据

            #region 装载 demo.flow
            if (isInstallFlowDemo == true)
            {
                BP.Port.Emp emp = new BP.Port.Emp("admin");
                BP.Web.WebUser.SignInOfGener(emp);

                //装载数据模版.
                BP.WF.DTS.LoadTemplete l = new BP.WF.DTS.LoadTemplete();
                string msg = l.Do() as string;

                //修复视图.
                Flow.RepareV_FlowData_View();
                
            }else{
                 
                //创建一个空白的流程，不然，整个结构就出问题。
                FlowSorts fss = new FlowSorts();
                fss.RetrieveAll();
                fss.Delete();

                FlowSort fs = new FlowSort();
                fs.Name = "流程树";
                fs.No = "01";
                fs.TreeNo = "01";
                fs.IsDir = true;
                fs.ParentNo = "0";
                fs.Insert();

                FlowSort s1 = (FlowSort)fs.DoCreateSubNode();
                s1.Name = "日常办公类";
                s1.Update();

                s1 = (FlowSort)fs.DoCreateSubNode();
                s1.Name = "财务类";
                s1.Update();

                s1 = (FlowSort)fs.DoCreateSubNode();
                s1.Name = "人力资源类";
                s1.Update();

                
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

            #region 执行补充的sql, 让外键的字段长度都设置成100.
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET maxlen=100 WHERE LGType=2 AND MaxLen<100");
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET maxlen=100 WHERE KeyOfEn='FK_Dept'");

            //Nodes nds = new Nodes();
            //nds.RetrieveAll();
            //foreach (Node nd in nds)
            //    nd.HisWork.CheckPhysicsTable();
            #endregion 执行补充的sql, 让外键的字段长度都设置成100.

            #region 安装ccim.
            Glo.DoInstallCCIM();
            #endregion

        }
        /// <summary>
        /// 安装CCIM
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="yunXingHuanjing"></param>
        /// <param name="isDemo"></param>
        public static void DoInstallCCIM()
        {
          //  string sqlscript = SystemConfig.PathOfWebApp + "\\WF\\Data\\Install\\SQLScript\\CCIM_" + BP.Sys.SystemConfig.AppCenterDBType + ".sql";
           // BP.DA.DBAccess.RunSQLScriptGo(sqlscript);
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
        /// <summary>
        /// 产生新的编号
        /// </summary>
        /// <param name="rptKey"></param>
        /// <returns></returns>
        public static string GenerFlowNo(string rptKey)
        {
            rptKey = rptKey.Replace("ND", "");
            rptKey = rptKey.Replace("Rpt", "");
            switch (rptKey.Length)
            {
                case 0:
                    return "001";
                case 1:
                    return "00" + rptKey;
                case 2:
                    return "0" + rptKey;
                case 3:
                    return rptKey;
                default:
                    return "001";
            }
            return rptKey;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool IsShowFlowNum
        {
            get
            {
                switch (SystemConfig.AppSettings["IsShowFlowNum"])
                {
                    case "1":
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// 产生word文档.
        /// </summary>
        /// <param name="wk"></param>
        public static void GenerWord(object filename, Work wk)
        {
            BP.WF.Glo.KillProcess("WINWORD.EXE");
            string enName = wk.EnMap.PhysicsTable;
            try
            {
                RegistryKey delKey = Registry.LocalMachine.OpenSubKey(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Shared Tools\Text Converters\Import\",
                    true);
                delKey.DeleteValue("MSWord6.wpc");
                delKey.Close();
            }
            catch
            {
            }

            GroupField currGF = new GroupField();
            MapAttrs mattrs = new MapAttrs(enName);
            GroupFields gfs = new GroupFields(enName);
            MapDtls dtls = new MapDtls(enName);
            foreach (MapDtl dtl in dtls)
                dtl.IsUse = false;

            // 计算出来单元格的行数。
            int rowNum = 0;
            foreach (GroupField gf in gfs)
            {
                rowNum++;
                bool isLeft = true;
                foreach (MapAttr attr in mattrs)
                {
                    if (attr.UIVisible == false)
                        continue;

                    if (attr.GroupID != gf.OID)
                        continue;

                    if (attr.UIIsLine)
                    {
                        rowNum++;
                        isLeft = true;
                        continue;
                    }

                    if (isLeft == false)
                        rowNum++;
                    isLeft = !isLeft;
                }
            }

            rowNum = rowNum + 2 + dtls.Count;

            // 创建Word文档
            string CheckedInfo = "";
            string message = "";
            Object Nothing = System.Reflection.Missing.Value;

            #region 没用代码
            //  object filename = fileName;

            //Word.Application WordApp = new Word.ApplicationClass();
            //Word.Document WordDoc = WordApp.Documents.Add(ref  Nothing, ref  Nothing, ref  Nothing, ref  Nothing);
            //try
            //{
            //    WordApp.ActiveWindow.View.Type = Word.WdViewType.wdOutlineView;
            //    WordApp.ActiveWindow.View.SeekView = Word.WdSeekView.wdSeekPrimaryHeader;

            //    #region 增加页眉
            //    // 添加页眉 插入图片
            //    string pict = SystemConfig.PathOfDataUser + "log.jpg"; // 图片所在路径
            //    if (System.IO.File.Exists(pict))
            //    {
            //        System.Drawing.Image img = System.Drawing.Image.FromFile(pict);
            //        object LinkToFile = false;
            //        object SaveWithDocument = true;
            //        object Anchor = WordDoc.Application.Selection.Range;
            //        WordDoc.Application.ActiveDocument.InlineShapes.AddPicture(pict, ref  LinkToFile,
            //            ref  SaveWithDocument, ref  Anchor);
            //        //    WordDoc.Application.ActiveDocument.InlineShapes[1].Width = img.Width; // 图片宽度
            //        //    WordDoc.Application.ActiveDocument.InlineShapes[1].Height = img.Height; // 图片高度
            //    }
            //    WordApp.ActiveWindow.ActivePane.Selection.InsertAfter("[驰骋业务流程管理系统 http://ccFlow.org]");
            //    WordApp.Selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft; // 设置右对齐
            //    WordApp.ActiveWindow.View.SeekView = Word.WdSeekView.wdSeekMainDocument; // 跳出页眉设置
            //    WordApp.Selection.ParagraphFormat.LineSpacing = 15f; // 设置文档的行间距
            //    #endregion

            //    // 移动焦点并换行
            //    object count = 14;
            //    object WdLine = Word.WdUnits.wdLine; // 换一行;
            //    WordApp.Selection.MoveDown(ref  WdLine, ref  count, ref  Nothing); // 移动焦点
            //    WordApp.Selection.TypeParagraph(); // 插入段落

            //    // 文档中创建表格
            //    Word.Table newTable = WordDoc.Tables.Add(WordApp.Selection.Range, rowNum, 4, ref  Nothing, ref  Nothing);

            //    // 设置表格样式
            //    newTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleThickThinLargeGap;
            //    newTable.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;

            //    newTable.Columns[1].Width = 100f;
            //    newTable.Columns[2].Width = 100f;
            //    newTable.Columns[3].Width = 100f;
            //    newTable.Columns[4].Width = 100f;

            //    // 填充表格内容
            //    newTable.Cell(1, 1).Range.Text = wk.EnDesc;
            //    newTable.Cell(1, 1).Range.Bold = 2; // 设置单元格中字体为粗体

            //    // 合并单元格
            //    newTable.Cell(1, 1).Merge(newTable.Cell(1, 4));
            //    WordApp.Selection.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter; // 垂直居中
            //    WordApp.Selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter; // 水平居中

            //    int groupIdx = 1;
            //    foreach (GroupField gf in gfs)
            //    {
            //        groupIdx++;
            //        // 填充表格内容
            //        newTable.Cell(groupIdx, 1).Range.Text = gf.Lab;
            //        newTable.Cell(groupIdx, 1).Range.Font.Color = Word.WdColor.wdColorDarkBlue; // 设置单元格内字体颜色
            //        newTable.Cell(groupIdx, 1).Shading.BackgroundPatternColor = Word.WdColor.wdColorGray25;
            //        // 合并单元格
            //        newTable.Cell(groupIdx, 1).Merge(newTable.Cell(groupIdx, 4));
            //        WordApp.Selection.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

            //        groupIdx++;

            //        bool isLeft = true;
            //        bool isColumns2 = false;
            //        int currColumnIndex = 0;
            //        foreach (MapAttr attr in mattrs)
            //        {
            //            if (attr.UIVisible == false)
            //                continue;

            //            if (attr.GroupID != gf.OID)
            //                continue;

            //            if (newTable.Rows.Count < groupIdx)
            //                continue;

            //            #region 增加从表
            //            foreach (MapDtl dtl in dtls)
            //            {
            //                if (dtl.IsUse)
            //                    continue;

            //                if (dtl.RowIdx != groupIdx - 3)
            //                    continue;

            //                if (gf.OID != dtl.GroupID)
            //                    continue;

            //                GEDtls dtlsDB = new GEDtls(dtl.No);
            //                QueryObject qo = new QueryObject(dtlsDB);
            //                switch (dtl.DtlOpenType)
            //                {
            //                    case DtlOpenType.ForEmp:
            //                        qo.AddWhere(GEDtlAttr.RefPK, wk.OID);
            //                        break;
            //                    case DtlOpenType.ForWorkID:
            //                        qo.AddWhere(GEDtlAttr.RefPK, wk.OID);
            //                        break;
            //                    case DtlOpenType.ForFID:
            //                        qo.AddWhere(GEDtlAttr.FID, wk.OID);
            //                        break;
            //                }
            //                qo.DoQuery();

            //                newTable.Rows[groupIdx].SetHeight(100f, Word.WdRowHeightRule.wdRowHeightAtLeast);
            //                newTable.Cell(groupIdx, 1).Merge(newTable.Cell(groupIdx, 4));

            //                Attrs dtlAttrs = dtl.GenerMap().Attrs;
            //                int colNum = 0;
            //                foreach (Attr attrDtl in dtlAttrs)
            //                {
            //                    if (attrDtl.UIVisible == false)
            //                        continue;
            //                    colNum++;
            //                }

            //                newTable.Cell(groupIdx, 1).Select();
            //                WordApp.Selection.Delete(ref Nothing, ref Nothing);
            //                Word.Table newTableDtl = WordDoc.Tables.Add(WordApp.Selection.Range, dtlsDB.Count + 1, colNum,
            //                    ref Nothing, ref Nothing);

            //                newTableDtl.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            //                newTableDtl.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;

            //                int colIdx = 1;
            //                foreach (Attr attrDtl in dtlAttrs)
            //                {
            //                    if (attrDtl.UIVisible == false)
            //                        continue;
            //                    newTableDtl.Cell(1, colIdx).Range.Text = attrDtl.Desc;
            //                    colIdx++;
            //                }

            //                int idxRow = 1;
            //                foreach (GEDtl item in dtlsDB)
            //                {
            //                    idxRow++;
            //                    int columIdx = 0;
            //                    foreach (Attr attrDtl in dtlAttrs)
            //                    {
            //                        if (attrDtl.UIVisible == false)
            //                            continue;
            //                        columIdx++;

            //                        if (attrDtl.IsFKorEnum)
            //                            newTableDtl.Cell(idxRow, columIdx).Range.Text = item.GetValRefTextByKey(attrDtl.Key);
            //                        else
            //                        {
            //                            if (attrDtl.MyDataType == DataType.AppMoney)
            //                                newTableDtl.Cell(idxRow, columIdx).Range.Text = item.GetValMoneyByKey(attrDtl.Key).ToString("0.00");
            //                            else
            //                                newTableDtl.Cell(idxRow, columIdx).Range.Text = item.GetValStrByKey(attrDtl.Key);

            //                            if (attrDtl.IsNum)
            //                                newTableDtl.Cell(idxRow, columIdx).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
            //                        }
            //                    }
            //                }

            //                groupIdx++;
            //                isLeft = true;
            //            }
            //            #endregion 增加从表

            //            if (attr.UIIsLine)
            //            {
            //                currColumnIndex = 0;
            //                isLeft = true;
            //                if (attr.IsBigDoc)
            //                {
            //                    newTable.Rows[groupIdx].SetHeight(100f, Word.WdRowHeightRule.wdRowHeightAtLeast);
            //                    newTable.Cell(groupIdx, 1).Merge(newTable.Cell(groupIdx, 4));
            //                    newTable.Cell(groupIdx, 1).Range.Text = attr.Name + ":\r\n" + wk.GetValStrByKey(attr.KeyOfEn);
            //                }
            //                else
            //                {
            //                    newTable.Cell(groupIdx, 2).Merge(newTable.Cell(groupIdx, 4));
            //                    newTable.Cell(groupIdx, 1).Range.Text = attr.Name;
            //                    newTable.Cell(groupIdx, 2).Range.Text = wk.GetValStrByKey(attr.KeyOfEn);
            //                }
            //                groupIdx++;
            //                continue;
            //            }
            //            else
            //            {
            //                if (attr.IsBigDoc)
            //                {
            //                    if (currColumnIndex == 2)
            //                    {
            //                        currColumnIndex = 0;
            //                    }

            //                    newTable.Rows[groupIdx].SetHeight(100f, Word.WdRowHeightRule.wdRowHeightAtLeast);
            //                    if (currColumnIndex == 0)
            //                    {
            //                        newTable.Cell(groupIdx, 1).Merge(newTable.Cell(groupIdx, 2));
            //                        newTable.Cell(groupIdx, 1).Range.Text = attr.Name + ":\r\n" + wk.GetValStrByKey(attr.KeyOfEn);
            //                        currColumnIndex = 3;
            //                        continue;
            //                    }
            //                    else if (currColumnIndex == 3)
            //                    {
            //                        newTable.Cell(groupIdx, 2).Merge(newTable.Cell(groupIdx, 3));
            //                        newTable.Cell(groupIdx, 2).Range.Text = attr.Name + ":\r\n" + wk.GetValStrByKey(attr.KeyOfEn);
            //                        currColumnIndex = 0;
            //                        groupIdx++;
            //                        continue;
            //                    }
            //                    else
            //                    {
            //                        continue;
            //                    }
            //                }
            //                else
            //                {
            //                    string s = "";
            //                    if (attr.LGType == FieldTypeS.Normal)
            //                    {
            //                        if (attr.MyDataType == DataType.AppMoney)
            //                            s = wk.GetValDecimalByKey(attr.KeyOfEn).ToString("0.00");
            //                        else
            //                            s = wk.GetValStrByKey(attr.KeyOfEn);
            //                    }
            //                    else
            //                    {
            //                        s = wk.GetValRefTextByKey(attr.KeyOfEn);
            //                    }

            //                    switch (currColumnIndex)
            //                    {
            //                        case 0:
            //                            newTable.Cell(groupIdx, 1).Range.Text = attr.Name;
            //                            if (attr.IsSigan)
            //                            {
            //                                string path = BP.Sys.SystemConfig.PathOfDataUser + "\\Siganture\\" + s + ".jpg";
            //                                if (System.IO.File.Exists(path))
            //                                {
            //                                    System.Drawing.Image img = System.Drawing.Image.FromFile(path);
            //                                    object LinkToFile = false;
            //                                    object SaveWithDocument = true;
            //                                    //object Anchor = WordDoc.Application.Selection.Range;
            //                                    object Anchor = newTable.Cell(groupIdx, 2).Range;

            //                                    WordDoc.Application.ActiveDocument.InlineShapes.AddPicture(path, ref  LinkToFile,
            //                                        ref  SaveWithDocument, ref  Anchor);
            //                                    //    WordDoc.Application.ActiveDocument.InlineShapes[1].Width = img.Width; // 图片宽度
            //                                    //    WordDoc.Application.ActiveDocument.InlineShapes[1].Height = img.Height; // 图片高度
            //                                }
            //                                else
            //                                {
            //                                    newTable.Cell(groupIdx, 2).Range.Text = s;
            //                                }
            //                            }
            //                            else
            //                            {
            //                                if (attr.IsNum)
            //                                {
            //                                    newTable.Cell(groupIdx, 2).Range.Text = s;
            //                                    newTable.Cell(groupIdx, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
            //                                }
            //                                else
            //                                {
            //                                    newTable.Cell(groupIdx, 2).Range.Text = s;
            //                                }
            //                            }
            //                            currColumnIndex = 1;
            //                            continue;
            //                            break;
            //                        case 1:
            //                            newTable.Cell(groupIdx, 3).Range.Text = attr.Name;
            //                            if (attr.IsSigan)
            //                            {
            //                                string path = BP.Sys.SystemConfig.PathOfDataUser + "\\Siganture\\" + s + ".jpg";
            //                                if (System.IO.File.Exists(path))
            //                                {
            //                                    System.Drawing.Image img = System.Drawing.Image.FromFile(path);
            //                                    object LinkToFile = false;
            //                                    object SaveWithDocument = true;
            //                                    object Anchor = newTable.Cell(groupIdx, 4).Range;
            //                                    WordDoc.Application.ActiveDocument.InlineShapes.AddPicture(path, ref  LinkToFile,
            //                                        ref  SaveWithDocument, ref  Anchor);
            //                                }
            //                                else
            //                                {
            //                                    newTable.Cell(groupIdx, 4).Range.Text = s;
            //                                }
            //                            }
            //                            else
            //                            {
            //                                if (attr.IsNum)
            //                                {
            //                                    newTable.Cell(groupIdx, 4).Range.Text = s;
            //                                    newTable.Cell(groupIdx, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
            //                                }
            //                                else
            //                                {
            //                                    newTable.Cell(groupIdx, 4).Range.Text = s;
            //                                }
            //                            }
            //                            currColumnIndex = 0;
            //                            groupIdx++;
            //                            continue;
            //                            break;
            //                        default:
            //                            break;
            //                    }
            //                }
            //            }
            //        }
            //    }  //结束循环

            //    #region 添加页脚
            //    WordApp.ActiveWindow.View.SeekView = Word.WdSeekView.wdSeekPrimaryFooter;
            //    WordApp.ActiveWindow.ActivePane.Selection.InsertAfter("模板由ccflow自动生成，严谨转载。此流程的详细内容请访问 http://doc.ccFlow.org。 建造流程管理系统请致电: 0531-82374939  ");
            //    WordApp.Selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
            //    #endregion

            //    // 文件保存
            //    WordDoc.SaveAs(ref  filename, ref  Nothing, ref  Nothing, ref  Nothing,
            //        ref  Nothing, ref  Nothing, ref  Nothing, ref  Nothing,
            //        ref  Nothing, ref  Nothing, ref  Nothing, ref  Nothing, ref  Nothing,
            //        ref  Nothing, ref  Nothing, ref  Nothing);

            //    WordDoc.Close(ref  Nothing, ref  Nothing, ref  Nothing);
            //    WordApp.Quit(ref  Nothing, ref  Nothing, ref  Nothing);
            //    try
            //    {
            //        string docFile = filename.ToString();
            //        string pdfFile = docFile.Replace(".doc", ".pdf");
            //        Glo.Rtf2PDF(docFile, pdfFile);
            //    }
            //    catch (Exception ex)
            //    {
            //        BP.DA.Log.DebugWriteInfo("@生成pdf失败." + ex.Message);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //    // WordApp.Quit(ref  Nothing, ref  Nothing, ref  Nothing);
            //    WordDoc.Close(ref  Nothing, ref  Nothing, ref  Nothing);
            //    WordApp.Quit(ref  Nothing, ref  Nothing, ref  Nothing);
            //}
            #endregion
        }
        #endregion 执行安装.

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
                str += GERptAttr.FK_NY + ",";
                str += GERptAttr.FlowDaySpan + ",";
                str += GERptAttr.FlowEmps + ",";
                str += GERptAttr.FlowEnder + ",";
                str += GERptAttr.FlowEnderRDT + ",";
                str += GERptAttr.FlowEndNode + ",";
                str += GERptAttr.FlowNote + ",";
                str += GERptAttr.FlowStarter + ",";
                str += GERptAttr.FlowStartRDT + ",";
                str += GERptAttr.GuestName + ",";
                str += GERptAttr.GuestNo + ",";
                str += GERptAttr.GUID + ",";
                str += GERptAttr.MyNum + ",";
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
                return str ;
               // return typeof(GERptAttr).GetFields().Select(o => o.Name).ToList();
            }
        }
        /// <summary>
        /// 根据文字处理抄送，与发送人
        /// </summary>
        /// <param name="note"></param>
        /// <param name="emps"></param>
        public static void DealNote(string note, BP.Port.Emps emps)
        {
            note = "请综合处阅知。李江龙核示。请王薇、田晓红批示。";
            note = note.Replace("阅知", "阅知@");

            note = note.Replace("请", "@");
            note = note.Replace("呈", "@");
            note = note.Replace("报", "@");
            string[] strs = note.Split('@');

            string ccTo = "";
            string sendTo = "";
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;

                if (str.Contains("阅知") == true
                    || str.Contains("阅度") == true)
                {
                    /*抄送的.*/
                    foreach (BP.Port.Emp emp in emps)
                    {
                        if (str.Contains(emp.No) == false)
                            continue;
                        ccTo += emp.No + ",";
                    }
                    continue;
                }

                if (str.Contains("阅处") == true
                  || str.Contains("阅办") == true)
                {
                    /*发送送的.*/
                    foreach (BP.Port.Emp emp in emps)
                    {
                        if (str.Contains(emp.No) == false)
                            continue;
                        sendTo += emp.No + ",";
                    }
                    continue;
                }
            }
        }

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
                ArrayList al = BP.En.ClassFactory.GetObjects("BP.WF.FlowEventBase");
                foreach (FlowEventBase en in al)
                {
                    Htable_FlowFEE.Add(en.ToString(), en);
                }
            }
            FlowEventBase myen = Htable_FlowFEE[enName] as FlowEventBase;
            if (myen == null)
                throw new Exception("@根据类名称获取流程事件实体实例出现错误:" + enName + ",没有找到该类的实体.");
            return myen;
        }
        /// <summary>
        /// 获得事件实体String，根据编号或者流程标记
        /// </summary>
        /// <param name="flowMark">流程标记</param>
        /// <param name="flowNo">流程编号</param>
        /// <returns>null, 或者流程实体.</returns>
        public static string GetFlowEventEntityStringByFlowMark(string flowMark, string flowNo)
        {
            FlowEventBase en =GetFlowEventEntityByFlowMark(  flowMark,   flowNo);
            if (en == null)
                return "";
            else
                return en.ToString();
        }
        /// <summary>
        /// 获得事件实体，根据编号或者流程标记.
        /// </summary>
        /// <param name="flowMark">流程标记</param>
        /// <param name="flowNo">流程编号</param>
        /// <returns>null, 或者流程实体.</returns>
        public static FlowEventBase GetFlowEventEntityByFlowMark(string flowMark, string flowNo)
        {
            if (Htable_FlowFEE == null || Htable_FlowFEE.Count == 0)
            {
                Htable_FlowFEE = new Hashtable();
                ArrayList al = BP.En.ClassFactory.GetObjects("BP.WF.FlowEventBase");
                Htable_FlowFEE.Clear();
                foreach (FlowEventBase en in al)
                {
                    Htable_FlowFEE.Add(en.ToString(), en);
                }
            }

            foreach (string key in Htable_FlowFEE.Keys)
            {
                FlowEventBase fee = Htable_FlowFEE[key] as FlowEventBase;
                if (fee.FlowMark == flowMark || fee.FlowMark==flowNo)
                    return fee;
            }
            return null;
        }
        #endregion 与流程事件实体相关.

        /// <summary>
        /// 执行发送工作后处理的业务逻辑
        /// 用于流程发送后事件调用.
        /// 如果处理失败，就会抛出异常.
        /// </summary>
        public static void DealBuinessAfterSendWork(string fk_flow, Int64 workid,
            string doFunc, string WorkIDs, string cFlowNo, int cNodeID, string cEmp)
        {
            if (doFunc == "SetParentFlow")
            {
                /* 如果需要设置子父流程信息.
                 * 应用于合并审批,当多个子流程合并审批,审批后发起一个父流程.
                 */
                string[] workids = WorkIDs.Split(',');
                string okworkids = ""; //成功发送后的workids.
                GenerWorkFlow gwf = new GenerWorkFlow();
                foreach (string id in workids)
                {
                    if (string.IsNullOrEmpty(id))
                        continue;

                    // 把数据copy到里面,让子流程也可以得到父流程的数据。
                    Int64 workidC = Int64.Parse(id);

                    //设置当前流程的ID
                    BP.WF.Dev2Interface.SetParentInfo(cFlowNo, workidC, fk_flow, workid, cNodeID, cEmp);

                    // 判断是否可以执行，不能执行也要发送下去.
                    gwf.WorkID = workidC;
                    if (gwf.RetrieveFromDBSources() == 0)
                        continue;

                    // 是否可以执行？
                    if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(gwf.FK_Flow, gwf.FK_Node, workidC, WebUser.No) == false)
                        continue;

                    //执行向下发送.
                    try
                    {
                        BP.WF.Dev2Interface.Node_SendWork(cFlowNo, workidC);
                        okworkids += workidC;
                    }
                    catch (Exception ex)
                    {
                        #region 如果有一个发送失败，就撤销子流程与父流程.
                        //首先把主流程撤销发送.
                        BP.WF.Dev2Interface.Flow_DoUnSend(fk_flow, workid);

                        //把已经发送成功的子流程撤销发送.
                        string[] myokwokid = okworkids.Split(',');
                        foreach (string okwokid in myokwokid)
                        {
                            if (string.IsNullOrEmpty(id))
                                continue;

                            // 把数据copy到里面,让子流程也可以得到父流程的数据。
                            workidC = Int64.Parse(id);
                            BP.WF.Dev2Interface.Flow_DoUnSend(cFlowNo, workidC);
                        }
                        #endregion 如果有一个发送失败，就撤销子流程与父流程.
                        throw new Exception("@在执行子流程(" + gwf.Title + ")发送时出现如下错误:" + ex.Message);
                    }
                }
            }

        }
        #endregion 全局的方法处理

        #region web.config 属性.
        /// <summary>
        /// 根据配置的信息不同，从不同的表里获取人员岗位信息。
        /// </summary>
        public static string EmpStation
        {
            get
            {
                if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneMore)
                    return "Port_DeptEmpStation";
                else
                    return "Port_EmpStation";
            }
        }
        public static string EmpDept
        {
            get
            {
                if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneMore)
                    return "Port_DeptEmp";
                else
                    return "Port_EmpDept";
            }
        }

        /// <summary>
        /// 是否admin
        /// </summary>
        public static bool IsAdmin
        {
            get
            {
                string s = BP.Sys.SystemConfig.AppSettings["adminers"];
                if (string.IsNullOrEmpty(s))
                    s = "admin,";
                return s.Contains(BP.Web.WebUser.No);
            }
        }
        /// <summary>
        /// 获取mapdata字段查询Like。
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="colName">列编号</param>
        public static string MapDataLikeKeyV1(string flowNo, string colName)
        {
            flowNo = int.Parse(flowNo).ToString();
            string len = BP.Sys.SystemConfig.AppCenterDBLengthStr;
            if (flowNo.Length == 1)
                return " " + colName + " LIKE 'ND" + flowNo + "%' AND " + len + "(" + colName + ")=5";
            if (flowNo.Length == 2)
                return " " + colName + " LIKE 'ND" + flowNo + "%' AND " + len + "(" + colName + ")=6";
            if (flowNo.Length == 3)
                return " " + colName + " LIKE 'ND" + flowNo + "%' AND " + len + "(" + colName + ")=7";

            return " " + colName + " LIKE 'ND" + flowNo + "%' AND " + len + "(" + colName + ")=8";
        }
        public static string MapDataLikeKey(string flowNo, string colName)
        {
            flowNo = int.Parse(flowNo).ToString();
            string len = BP.Sys.SystemConfig.AppCenterDBLengthStr;
            //edited by liuxc,2016-02-22,合并逻辑，原来分流程编号的位数，现在统一处理
            return " (" + colName + " LIKE 'ND" + flowNo + "%' AND " + len + "(" + colName + ")=" +
                   ("ND".Length + flowNo.Length + 2) + ") OR (" + colName +
                   " = 'ND" + flowNo + "Rpt' ) OR (" + colName + " LIKE 'ND" + flowNo + "__Dtl%' AND " + len + "(" +
                   colName + ")>" + ("ND".Length + flowNo.Length + 2 + "Dtl".Length) + ")";
        }
        /// <summary>
        /// 短信时间发送从
        /// 默认从 8 点开始.
        /// </summary>
        public static int SMSSendTimeFromHour
        {
            get
            {
                try
                {
                    return int.Parse(BP.Sys.SystemConfig.AppSettings["SMSSendTimeFromHour"]);
                }
                catch
                {
                    return 8;
                }
            }
        }
        /// <summary>
        /// 短信时间发送到
        /// 默认到 20 点结束.
        /// </summary>
        public static int SMSSendTimeToHour
        {
            get
            {
                try
                {
                    return int.Parse(BP.Sys.SystemConfig.AppSettings["SMSSendTimeToHour"]);
                }
                catch
                {
                    return 8;
                }
            }
        }
        #endregion webconfig属性.

        #region 常用方法
        private static string html = "";
        private static ArrayList htmlArr = new ArrayList();
        private static string backHtml = "";
        private static Int64 workid = 0;
        /// <summary>
        /// 模拟运行
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="empNo">要执行的人员.</param>
        /// <returns>执行信息.</returns>
        public static string Simulation_RunOne(string flowNo, string empNo, string paras)
        {
            backHtml = "";//需要重新赋空值
            Hashtable ht = null;
            if (string.IsNullOrEmpty(paras) == false)
            {
                AtPara ap = new AtPara(paras);
                ht = ap.HisHT;
            }

            Emp emp = new Emp(empNo);
            backHtml += " **** 开始使用:" + Glo.GenerUserImgSmallerHtml(emp.No, emp.Name) + "登录模拟执行工作流程";
            BP.WF.Dev2Interface.Port_Login(empNo);

            workid = BP.WF.Dev2Interface.Node_CreateBlankWork(flowNo, ht, null, emp.No, null);
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(flowNo, workid, ht);
            backHtml += objs.ToMsgOfHtml().Replace("@", "<br>@");  //记录消息.


            string[] accepters = objs.VarAcceptersID.Split(',');


            foreach (string acce in accepters)
            {
                if (string.IsNullOrEmpty(acce) == true)
                    continue;

                // 执行发送.
                Simulation_Run_S1(flowNo, workid, acce, ht, empNo);
                break;
            }
            //return html;
            //return htmlArr;
            return backHtml;
        }
        private static bool isAdd = true;
        private static void Simulation_Run_S1(string flowNo, Int64 workid, string empNo, Hashtable ht, string beginEmp)
        {
            //htmlArr.Add(html);
            Emp emp = new Emp(empNo);
            //html = "";
            backHtml += "empNo" + beginEmp;
            backHtml += "<br> **** 让:" + Glo.GenerUserImgSmallerHtml(emp.No, emp.Name) + "执行模拟登录. ";
            // 让其登录.
            BP.WF.Dev2Interface.Port_Login(empNo);

            //执行发送.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(flowNo, workid, ht);
            backHtml += "<br>" + objs.ToMsgOfHtml().Replace("@", "<br>@");

            if (objs.VarAcceptersID == null)
            {
                isAdd = false;
                backHtml += " <br> **** 流程结束,查看<a href='/WF/WFRpt.aspx?WorkID=" + workid + "&FK_Flow=" + flowNo + "' target=_blank >流程轨迹</a> ====";
                //htmlArr.Add(html);
                //backHtml += "nextEmpNo";
                return;
            }

            if (string.IsNullOrEmpty(objs.VarAcceptersID))//此处添加为空判断，跳过下面方法的执行，否则出错。
            {
                return;
            }
            string[] accepters = objs.VarAcceptersID.Split(',');

            foreach (string acce in accepters)
            {
                if (string.IsNullOrEmpty(acce) == true)
                    continue;

                //执行发送.
                Simulation_Run_S1(flowNo, workid, acce, ht, beginEmp);
                break; //就不让其执行了.
            }
        }
        /// <summary>
        /// 是否手机访问?
        /// </summary>
        /// <returns></returns>
        public static bool IsMobile()
        {
            if (SystemConfig.IsBSsystem == false)
                return false;

            string agent = (BP.Sys.Glo.Request.UserAgent + "").ToLower().Trim();
            if (agent == "" || agent.IndexOf("mozilla") != -1 || agent.IndexOf("opera") != -1)
                return false;
            return true;
        }
        /// <summary>
        /// 产生单据编号
        /// </summary>
        /// <param name="billFormat"></param>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string GenerBillNo(string billNo, Int64 workid, Entity en, string flowPTable)
        {
            if (string.IsNullOrEmpty(billNo))
                return "";
            if (billNo.Contains("@"))
                billNo = BP.WF.Glo.DealExp(billNo, en, null);

            /*如果，Bill 有规则 */
            billNo = billNo.Replace("{YYYY}", DateTime.Now.ToString("yyyy"));
            billNo = billNo.Replace("{yyyy}", DateTime.Now.ToString("yyyy"));

            billNo = billNo.Replace("{yy}", DateTime.Now.ToString("yy"));
            billNo = billNo.Replace("{YY}", DateTime.Now.ToString("YY"));

            billNo = billNo.Replace("{MM}", DateTime.Now.ToString("MM"));
            billNo = billNo.Replace("{mm}", DateTime.Now.ToString("mm"));

            billNo = billNo.Replace("{DD}", DateTime.Now.ToString("DD"));
            billNo = billNo.Replace("{dd}", DateTime.Now.ToString("dd"));
            billNo = billNo.Replace("{HH}", DateTime.Now.ToString("HH"));
            billNo = billNo.Replace("{hh}", DateTime.Now.ToString("HH"));

            billNo = billNo.Replace("{LSH}", workid.ToString());
            billNo = billNo.Replace("{WorkID}", workid.ToString());
            billNo = billNo.Replace("{OID}", workid.ToString());

            if (billNo.Contains("@WebUser.DeptZi"))
            {
                string val = DBAccess.RunSQLReturnStringIsNull("SELECT Zi FROM Port_Dept where no='" + WebUser.FK_Dept + "'", "");
                billNo = billNo.Replace("@WebUser.DeptZi", val.ToString());
            }

            if (billNo.Contains("{ParentBillNo}"))
            {
                string pWorkID = DBAccess.RunSQLReturnStringIsNull("SELECT PWorkID FROM " + flowPTable + " WHERE OID=" + workid, "0");
                string parentBillNo = DBAccess.RunSQLReturnStringIsNull("SELECT BillNo FROM WF_GenerWorkFlow WHERE WorkID=" + pWorkID, "");
                billNo = billNo.Replace("{ParentBillNo}", parentBillNo);

                string sql = "";
                int num = 0;
                for (int i = 2; i < 7; i++)
                {
                    if (billNo.Contains("{LSH" + i + "}") == false)
                        continue;

                    sql = "SELECT COUNT(OID) FROM " + flowPTable + " WHERE PWorkID =" + pWorkID;
                    num = BP.DA.DBAccess.RunSQLReturnValInt(sql, 0);
                    billNo = billNo + num.ToString().PadLeft(i, '0');
                    billNo = billNo.Replace("{LSH" + i + "}", "");
                    break;
                }
            }
            else
            {
                string sql = "";
                int num = 0;
                for (int i = 2; i < 7; i++)
                {
                    if (billNo.Contains("{LSH" + i + "}") == false)
                        continue;

                    billNo = billNo.Replace("{LSH" + i + "}", "");
                    
                    sql = "SELECT COUNT(*) FROM " + flowPTable + " WHERE BillNo LIKE '" + billNo + "%'";
                    if (DBAccess.AppCenterDBType == DBType.MSSQL)
                    {
                        //改为取最大值
                        sql = " SELECT isnull(convert(int,max(RIGHT(billno,len(billno)-len('" + billNo + "')-1))),0) FROM "
                            + flowPTable + " WHERE BillNo LIKE '" + billNo + "%'";
                    }


                    num = BP.DA.DBAccess.RunSQLReturnValInt(sql, 0) + 1;
                    billNo = billNo + num.ToString().PadLeft(i, '0');
                }
            }
            //if (billNo.Contains("@") == true)
            //    billNo = Glo.DealExp(billNo, en, null);

            return billNo;
        }
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
        public static void AddToTrack(ActionType at, string flowNo, Int64 workID, Int64 fid, int fromNodeID, string fromNodeName, string fromEmpID, string fromEmpName,
            int toNodeID, string toNodeName, string toEmpID, string toEmpName, string note, string tag)
        {
            if (toNodeID == 0)
            {
                toNodeID = fromNodeID;
                toNodeName = fromNodeName;
            }

            Track t = new Track();
            t.WorkID = workID;
            t.FID = fid;
            t.RDT = DataType.CurrentDataTimess;
            t.HisActionType = at;

            t.NDFrom = fromNodeID;
            t.NDFromT = fromNodeName;

            t.EmpFrom = fromEmpID;
            t.EmpFromT = fromEmpName;
            t.FK_Flow = flowNo;

            t.NDTo = toNodeID;
            t.NDToT = toNodeName;

            t.EmpTo = toEmpID;
            t.EmpToT = toEmpName;
            t.Msg = note;

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
        }
        /// <summary>
        /// 计算表达式是否通过(或者是否正确.)
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <param name="en">实体</param>
        /// <returns>true/false</returns>
        public static bool ExeExp(string exp, Entity en)
        {
            exp = exp.Replace("@WebUser.No", WebUser.No);
            exp = exp.Replace("@WebUser.Name", WebUser.Name);
            exp = exp.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
            exp = exp.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);


            string[] strs = exp.Split(' ');
            bool isPass = false;

            string key = strs[0].Trim();
            string oper = strs[1].Trim();
            string val = strs[2].Trim();
            val = val.Replace("'", "");
            val = val.Replace("%", "");
            val = val.Replace("~", "");
            BP.En.Row row = en.Row;
            foreach (string item in row.Keys)
            {
                if (key != item.Trim())
                    continue;

                string valPara = row[key].ToString();
                if (oper == "=")
                {
                    if (valPara == val)
                        return true;
                }

                if (oper.ToUpper() == "LIKE")
                {
                    if (valPara.Contains(val))
                        return true;
                }

                if (oper == ">")
                {
                    if (float.Parse(valPara) > float.Parse(val))
                        return true;
                }
                if (oper == ">=")
                {
                    if (float.Parse(valPara) >= float.Parse(val))
                        return true;
                }
                if (oper == "<")
                {
                    if (float.Parse(valPara) < float.Parse(val))
                        return true;
                }
                if (oper == "<=")
                {
                    if (float.Parse(valPara) <= float.Parse(val))
                        return true;
                }

                if (oper == "!=")
                {
                    if (float.Parse(valPara) != float.Parse(val))
                        return true;
                }

                throw new Exception("@参数格式错误:" + exp + " Key=" + key + " oper=" + oper + " Val=" + val);
            }

            return false;
        }
        /// <summary>
        /// 执行PageLoad装载数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="en"></param>
        /// <param name="mattrs"></param>
        /// <param name="dtls"></param>
        /// <returns></returns>
        public static Entity DealPageLoadFull(Entity en, MapExt item, MapAttrs mattrs, MapDtls dtls)
        {
            if (item == null)
                return en;

            DataTable dt = null;
            string sql = item.Tag;
            if (string.IsNullOrEmpty(sql) == false)
            {
                /* 如果有填充主表的sql  */
                sql = Glo.DealExp(sql, en, null);

                if (string.IsNullOrEmpty(sql) == false)
                {
                    if (sql.Contains("@"))
                        throw new Exception("设置的sql有错误可能有没有替换的变量:" + sql);
                    dt = DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count == 1)
                    {
                        DataRow dr = dt.Rows[0];
                        foreach (DataColumn dc in dt.Columns)
                        {
                            //去掉一些不需要copy的字段.
                            switch (dc.ColumnName)
                            {
                                case WorkAttr.OID:
                                case WorkAttr.FID:
                                case WorkAttr.Rec:
                                case WorkAttr.MD5:
                                case WorkAttr.MyNum:
                                case WorkAttr.RDT:
                                case "RefPK":
                                case WorkAttr.RecText:
                                    continue;
                                default:
                                    break;
                            }

                            if (string.IsNullOrEmpty(en.GetValStringByKey(dc.ColumnName)) || en.GetValStringByKey(dc.ColumnName)=="0")
                                en.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(item.Tag1)
                || item.Tag1.Length < 15)
                return en;

            // 填充从表.
            foreach (MapDtl dtl in dtls)
            {
                string[] sqls = item.Tag1.Split('*');
                foreach (string mysql in sqls)
                {
                    if (string.IsNullOrEmpty(mysql))
                        continue;
                    if (mysql.Contains(dtl.No + "=") == false)
                        continue;
                    if (mysql.Equals(dtl.No + "=") == true)
                        continue;

                    #region 处理sql.
                    sql = Glo.DealExp(mysql, en, null);
                    #endregion 处理sql.

                    if (string.IsNullOrEmpty(sql))
                        continue;

                    if (sql.Contains("@"))
                        throw new Exception("设置的sql有错误可能有没有替换的变量:" + sql);

                    GEDtls gedtls = null;

                    try
                    {
                        gedtls = new GEDtls(dtl.No);
                        gedtls.Delete(GEDtlAttr.RefPK, en.PKVal);
                    }
                    catch (Exception ex)
                    {
                        (gedtls.GetNewEntity as GEDtl).CheckPhysicsTable();
                    }

                    dt =
                        DBAccess.RunSQLReturnTable(sql.StartsWith(dtl.No + "=")
                                                       ? sql.Substring((dtl.No + "=").Length)
                                                       : sql);
                    foreach (DataRow dr in dt.Rows)
                    {
                        GEDtl gedtl = gedtls.GetNewEntity as GEDtl;
                        foreach (DataColumn dc in dt.Columns)
                        {
                            gedtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());
                        }

                        gedtl.RefPK = en.PKVal.ToString();
                        gedtl.RDT = DataType.CurrentDataTime;
                        gedtl.Rec = WebUser.No;
                        gedtl.Insert();
                    }
                }
            }
            return en;
        }
        /// <summary>
        /// 处理表达式
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <param name="en">数据源</param>
        /// <param name="errInfo">错误</param>
        /// <returns></returns>
        public static string DealExp(string exp, Entity en, string errInfo)
        {
            exp = exp.Replace("~", "'");

            //首先替换加; 的。
            exp = exp.Replace("@WebUser.No;", WebUser.No);
            exp = exp.Replace("@WebUser.Name;", WebUser.Name);
            exp = exp.Replace("@WebUser.FK_Dept;", WebUser.FK_Dept);
            exp = exp.Replace("@WebUser.FK_DeptName;", WebUser.FK_DeptName);

            // 替换没有 ; 的 .
            exp = exp.Replace("@WebUser.No", WebUser.No);
            exp = exp.Replace("@WebUser.Name", WebUser.Name);
            exp = exp.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
            exp = exp.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);

            if (exp.Contains("@") == false)
            {
                exp = exp.Replace("~", "'");
                return exp;
            }

            //增加对新规则的支持. @MyField; 格式.
            foreach (Attr item in en.EnMap.Attrs)
            {
                if (exp.Contains("@" + item.Key + ";"))
                    exp = exp.Replace("@" + item.Key + ";", en.GetValStrByKey(item.Key));
            }
            if (exp.Contains("@") == false)
                return exp;

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
                {
                    key = key.Replace(",", "");
                    exp = exp.Replace("@" + key, en.GetValStrByKey(key));
                }
                else
                {
                    exp = exp.Replace("@" + key, en.GetValStrByKey(key));
                }
            }

            // 处理Para的替换.
            if (exp.Contains("@") && Glo.SendHTOfTemp != null)
            {
                foreach (string key in Glo.SendHTOfTemp.Keys)
                    exp = exp.Replace("@" + key, Glo.SendHTOfTemp[key].ToString());
            }

            if (exp.Contains("@") && SystemConfig.IsBSsystem == true)
            {
                /*如果是bs*/
                foreach (string key in System.Web.HttpContext.Current.Request.QueryString.Keys)
                    exp = exp.Replace("@" + key, System.Web.HttpContext.Current.Request.QueryString[key]);
            }
            #endregion

            exp = exp.Replace("~", "'");
            //exp = exp.Replace("''", "'");
            //exp = exp.Replace("''", "'");
            //exp = exp.Replace("=' ", "=''");
            //exp = exp.Replace("= ' ", "=''");
            return exp;
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
                    case WorkAttr.RDT:
                    case WorkAttr.CDT:
                    case WorkAttr.Rec:
                    case StartWorkAttr.Title:
                    case StartWorkAttr.Emps:
                    case StartWorkAttr.FK_Dept:
                    case StartWorkAttr.PRI:
                    case StartWorkAttr.FID:
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
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(s, "MD5").ToLower();
        }
        /// <summary>
        /// 装载流程数据 
        /// </summary>
        /// <param name="xlsFile"></param>
        public static string LoadFlowDataWithToSpecNode(string xlsFile)
        {
            DataTable dt = BP.DA.DBLoad.GetTableByExt(xlsFile);
            string err = "";
            string info = "";

            foreach (DataRow dr in dt.Rows)
            {
                string flowPK = dr["FlowPK"].ToString();
                string starter = dr["Starter"].ToString();
                string executer = dr["Executer"].ToString();
                int toNode = int.Parse(dr["ToNodeID"].ToString().Replace("ND", ""));
                Node nd = new Node();
                nd.NodeID = toNode;
                if (nd.RetrieveFromDBSources() == 0)
                {
                    err += "节点ID错误:" + toNode;
                    continue;
                }
                string sql = "SELECT count(*) as Num FROM ND" + int.Parse(nd.FK_Flow) + "01 WHERE FlowPK='" + flowPK + "'";
                int i = DBAccess.RunSQLReturnValInt(sql);
                if (i == 1)
                    continue; // 此数据已经调度了。

                #region 检查数据是否完整。
                BP.Port.Emp emp = new BP.Port.Emp();
                emp.No = executer;
                if (emp.RetrieveFromDBSources() == 0)
                {
                    err += "@账号:" + starter + ",不存在。";
                    continue;
                }
                if (string.IsNullOrEmpty(emp.FK_Dept))
                {
                    err += "@账号:" + starter + ",没有部门。";
                    continue;
                }

                emp.No = starter;
                if (emp.RetrieveFromDBSources() == 0)
                {
                    err += "@账号:" + executer + ",不存在。";
                    continue;
                }
                if (string.IsNullOrEmpty(emp.FK_Dept))
                {
                    err += "@账号:" + executer + ",没有部门。";
                    continue;
                }
                #endregion 检查数据是否完整。

                BP.Web.WebUser.SignInOfGener(emp);
                Flow fl = nd.HisFlow;
                Work wk = fl.NewWork();

                Attrs attrs = wk.EnMap.Attrs;
                //foreach (Attr attr in wk.EnMap.Attrs)
                //{
                //}

                foreach (DataColumn dc in dt.Columns)
                {
                    Attr attr = attrs.GetAttrByKey(dc.ColumnName.Trim());
                    if (attr == null)
                        continue;

                    string val = dr[dc.ColumnName].ToString().Trim();
                    switch (attr.MyDataType)
                    {
                        case DataType.AppString:
                        case DataType.AppDate:
                        case DataType.AppDateTime:
                            wk.SetValByKey(attr.Key, val);
                            break;
                        case DataType.AppInt:
                        case DataType.AppBoolean:
                            wk.SetValByKey(attr.Key, int.Parse(val));
                            break;
                        case DataType.AppMoney:
                        case DataType.AppDouble:
                        case DataType.AppRate:
                        case DataType.AppFloat:
                            wk.SetValByKey(attr.Key, decimal.Parse(val));
                            break;
                        default:
                            wk.SetValByKey(attr.Key, val);
                            break;
                    }
                }

                wk.SetValByKey(WorkAttr.Rec, BP.Web.WebUser.No);
                wk.SetValByKey(StartWorkAttr.FK_Dept, BP.Web.WebUser.FK_Dept);
                wk.SetValByKey("FK_NY", DataType.CurrentYearMonth);
                wk.SetValByKey(WorkAttr.MyNum, 1);
                wk.Update();

                Node ndStart = nd.HisFlow.HisStartNode;
                WorkNode wn = new WorkNode(wk, ndStart);
                try
                {
                    info += "<hr>" + wn.NodeSend(nd, executer).ToMsgOfHtml();
                }
                catch (Exception ex)
                {
                    err += "<hr>" + ex.Message;
                    WorkFlow wf = new WorkFlow(fl, wk.OID);
                    wf.DoDeleteWorkFlowByReal(true);
                    continue;
                }

                #region 更新 下一个节点数据。
                Work wkNext = nd.HisWork;
                wkNext.OID = wk.OID;
                wkNext.RetrieveFromDBSources();
                attrs = wkNext.EnMap.Attrs;
                foreach (DataColumn dc in dt.Columns)
                {
                    Attr attr = attrs.GetAttrByKey(dc.ColumnName.Trim());
                    if (attr == null)
                        continue;

                    string val = dr[dc.ColumnName].ToString().Trim();
                    switch (attr.MyDataType)
                    {
                        case DataType.AppString:
                        case DataType.AppDate:
                        case DataType.AppDateTime:
                            wkNext.SetValByKey(attr.Key, val);
                            break;
                        case DataType.AppInt:
                        case DataType.AppBoolean:
                            wkNext.SetValByKey(attr.Key, int.Parse(val));
                            break;
                        case DataType.AppMoney:
                        case DataType.AppDouble:
                        case DataType.AppRate:
                        case DataType.AppFloat:
                            wkNext.SetValByKey(attr.Key, decimal.Parse(val));
                            break;
                        default:
                            wkNext.SetValByKey(attr.Key, val);
                            break;
                    }
                }

                wkNext.DirectUpdate();

                GERpt rtp = fl.HisGERpt;
                rtp.SetValByKey("OID", wkNext.OID);
                rtp.RetrieveFromDBSources();
                rtp.Copy(wkNext);
                rtp.DirectUpdate();

                #endregion 更新 下一个节点数据。
            }
            return info + err;
        }
        public static string LoadFlowDataWithToSpecEndNode(string xlsFile)
        {
            DataTable dt = BP.DA.DBLoad.GetTableByExt(xlsFile);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            ds.WriteXml("C:\\已完成.xml");

            string err = "";
            string info = "";
            int idx = 0;
            foreach (DataRow dr in dt.Rows)
            {
                string flowPK = dr["FlowPK"].ToString().Trim();
                if (string.IsNullOrEmpty(flowPK))
                    continue;

                string starter = dr["Starter"].ToString();
                string executer = dr["Executer"].ToString();
                int toNode = int.Parse(dr["ToNodeID"].ToString().Replace("ND", ""));
                Node ndOfEnd = new Node();
                ndOfEnd.NodeID = toNode;
                if (ndOfEnd.RetrieveFromDBSources() == 0)
                {
                    err += "节点ID错误:" + toNode;
                    continue;
                }

                if (ndOfEnd.IsEndNode == false)
                {
                    err += "节点ID错误:" + toNode + ", 非结束节点。";
                    continue;
                }

                string sql = "SELECT count(*) as Num FROM ND" + int.Parse(ndOfEnd.FK_Flow) + "01 WHERE FlowPK='" + flowPK + "'";
                int i = DBAccess.RunSQLReturnValInt(sql);
                if (i == 1)
                    continue; // 此数据已经调度了。

                #region 检查数据是否完整。
                //发起人发起。
                BP.Port.Emp emp = new BP.Port.Emp();
                emp.No = executer;
                if (emp.RetrieveFromDBSources() == 0)
                {
                    err += "@账号:" + starter + ",不存在。";
                    continue;
                }

                if (string.IsNullOrEmpty(emp.FK_Dept))
                {
                    err += "@账号:" + starter + ",没有设置部门。";
                    continue;
                }

                emp = new BP.Port.Emp();
                emp.No = starter;
                if (emp.RetrieveFromDBSources() == 0)
                {
                    err += "@账号:" + starter + ",不存在。";
                    continue;
                }
                else
                {
                    emp.RetrieveFromDBSources();
                    if (string.IsNullOrEmpty(emp.FK_Dept))
                    {
                        err += "@账号:" + starter + ",没有设置部门。";
                        continue;
                    }
                }
                #endregion 检查数据是否完整。


                BP.Web.WebUser.SignInOfGener(emp);
                Flow fl = ndOfEnd.HisFlow;
                Work wk = fl.NewWork();
                foreach (DataColumn dc in dt.Columns)
                    wk.SetValByKey(dc.ColumnName.Trim(), dr[dc.ColumnName].ToString().Trim());

                wk.SetValByKey(WorkAttr.Rec, BP.Web.WebUser.No);
                wk.SetValByKey(StartWorkAttr.FK_Dept, BP.Web.WebUser.FK_Dept);
                wk.SetValByKey("FK_NY", DataType.CurrentYearMonth);
                wk.SetValByKey(WorkAttr.MyNum, 1);
                wk.Update();

                Node ndStart = fl.HisStartNode;
                WorkNode wn = new WorkNode(wk, ndStart);
                try
                {
                    info += "<hr>" + wn.NodeSend(ndOfEnd, executer).ToMsgOfHtml();
                }
                catch (Exception ex)
                {
                    err += "<hr>启动错误:" + ex.Message;
                    DBAccess.RunSQL("DELETE FROM ND" + int.Parse(ndOfEnd.FK_Flow) + "01 WHERE FlowPK='" + flowPK + "'");
                    WorkFlow wf = new WorkFlow(fl, wk.OID);
                    wf.DoDeleteWorkFlowByReal(true);
                    continue;
                }

                //结束点结束。
                emp = new BP.Port.Emp(executer);
                BP.Web.WebUser.SignInOfGener(emp);

                Work wkEnd = ndOfEnd.GetWork(wk.OID);
                foreach (DataColumn dc in dt.Columns)
                    wkEnd.SetValByKey(dc.ColumnName.Trim(), dr[dc.ColumnName].ToString().Trim());

                wkEnd.SetValByKey(WorkAttr.Rec, BP.Web.WebUser.No);
                wkEnd.SetValByKey(StartWorkAttr.FK_Dept, BP.Web.WebUser.FK_Dept);
                wkEnd.SetValByKey("FK_NY", DataType.CurrentYearMonth);
                wkEnd.SetValByKey(WorkAttr.MyNum, 1);
                wkEnd.Update();

                try
                {
                    WorkNode wnEnd = new WorkNode(wkEnd, ndOfEnd);
                    //  wnEnd.AfterNodeSave();
                    info += "<hr>" + wnEnd.NodeSend().ToMsgOfHtml();
                }
                catch (Exception ex)
                {
                    err += "<hr>结束错误(系统直接删除它):" + ex.Message;
                    WorkFlow wf = new WorkFlow(fl, wk.OID);
                    wf.DoDeleteWorkFlowByReal(true);
                    continue;
                }
            }
            return info + err;
        }
        /// <summary>
        /// 判断是否登陆当前UserNo
        /// </summary>
        /// <param name="userNo"></param>
        public static void IsSingleUser(string userNo)
        {
            if (string.IsNullOrEmpty(WebUser.No) || WebUser.No != userNo)
            {
                if (!string.IsNullOrEmpty(userNo))
                {
                    BP.WF.Dev2Interface.Port_Login(userNo);
                }
            }
        }
        //public static void ResetFlowView()
        //{
        //    string sql = "DROP VIEW V_WF_Data ";
        //    try
        //    {
        //        BP.DA.DBAccess.RunSQL(sql);
        //    }
        //    catch
        //    {
        //    }

        //    Flows fls = new Flows();
        //    fls.RetrieveAll();
        //    sql = "CREATE VIEW V_WF_Data AS ";
        //    foreach (Flow fl in fls)
        //    {
        //        fl.CheckRpt();
        //        sql += "\t\n SELECT '" + fl.No + "' as FK_Flow, '" + fl.Name + "' AS FlowName, '" + fl.FK_FlowSort + "' as FK_FlowSort,CDT,Emps,FID,FK_Dept,FK_NY,";
        //        sql += "MyNum,OID,RDT,Rec,Title,WFState,FlowEmps,";
        //        sql += "FlowStarter,FlowStartRDT,FlowEnder,FlowEnderRDT,FlowDaySpan FROM ND" + int.Parse(fl.No) + "Rpt";
        //        sql += "\t\n  UNION";
        //    }
        //    sql = sql.Substring(0, sql.Length - 6);
        //    sql += "\t\n GO";
        //    BP.DA.DBAccess.RunSQL(sql);
        //}
        public static void Rtf2PDF(object pathOfRtf, object pathOfPDF)
        {
            //        Object Nothing = System.Reflection.Missing.Value;
            //        //创建一个名为WordApp的组件对象    
            //        Microsoft.Office.Interop.Word.Application wordApp =
            //new Microsoft.Office.Interop.Word.ApplicationClass();
            //        //创建一个名为WordDoc的文档对象并打开    
            //        Microsoft.Office.Interop.Word.Document doc = wordApp.Documents.Open(ref pathOfRtf, ref Nothing, ref Nothing, ref Nothing, ref Nothing,
            // ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing,
            //ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing);

            //        //设置保存的格式    
            //        object filefarmat = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF;

            //        //保存为PDF    
            //        doc.SaveAs(ref pathOfPDF, ref filefarmat, ref Nothing, ref Nothing, ref Nothing, ref Nothing,
            //ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing,
            // ref Nothing, ref Nothing, ref Nothing);
            //        //关闭文档对象    
            //        doc.Close(ref Nothing, ref Nothing, ref Nothing);
            //        //推出组建    
            //        wordApp.Quit(ref Nothing, ref Nothing, ref Nothing);
            //        GC.Collect();
        }
        #endregion 常用方法

        #region 属性
        /// <summary>
        /// 消息
        /// </summary>
        public static string SessionMsg
        {
            get
            {
                Paras p = new Paras();
                p.SQL = "SELECT Msg FROM WF_Emp where No=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";
                p.AddFK_Emp();
                return DBAccess.RunSQLReturnString(p);

                //string SQL = "SELECT Msg FROM WF_Emp where No='"+BP.Web.WebUser.No+"'";
                //return DBAccess.RunSQLReturnString(SQL);
            }
            set
            {
                if (string.IsNullOrEmpty(value) == true)
                    return;

                Paras p = new Paras();
                p.SQL = "UPDATE WF_Emp SET Msg=" + SystemConfig.AppCenterDBVarStr + "v WHERE No=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";
                p.Add("v", value);
                p.AddFK_Emp();

                int i = DBAccess.RunSQL(p);
                if (i == 0)
                {
                    /*如果没有更新到.*/
                    BP.WF.Port.WFEmp emp = new Port.WFEmp();
                    emp.No = BP.Web.WebUser.No;
                    emp.Name = BP.Web.WebUser.Name;
                    emp.FK_Dept = BP.Web.WebUser.FK_Dept;
                    emp.Insert();
                    DBAccess.RunSQL(p);
                }

                //string SQL = "UPDATE WF_Emp SET Msg='" + value + "' WHERE No='" + BP.Web.WebUser.No + "'";
                //DBAccess.RunSQL(SQL);
            }
        }

        private static string _FromPageType = null;
        public static string FromPageType
        {
            get
            {
                _FromPageType = null;
                if (_FromPageType == null)
                {
                    try
                    {
                        string url = BP.Sys.Glo.Request.RawUrl;
                        int i = url.LastIndexOf("/") + 1;
                        int i2 = url.IndexOf(".aspx") - 6;

                        url = url.Substring(i);
                        url = url.Substring(0, url.IndexOf(".aspx"));
                        _FromPageType = url;
                        if (_FromPageType.Contains("SmallSingle"))
                            _FromPageType = "SmallSingle";
                        else if (_FromPageType.Contains("Small"))
                            _FromPageType = "Small";
                        else
                            _FromPageType = "";
                    }
                    catch (Exception ex)
                    {
                        _FromPageType = "";
                        //  throw new Exception(ex.Message + url + " i=" + i + " i2=" + i2);
                    }
                }
                return _FromPageType;
            }
        }
        private static Hashtable _SendHTOfTemp = null;
        /// <summary>
        /// 临时的发送传输变量.
        /// </summary>
        public static Hashtable SendHTOfTemp
        {
            get
            {
                if (_SendHTOfTemp == null)
                    _SendHTOfTemp = new Hashtable();
                return _SendHTOfTemp[BP.Web.WebUser.No] as Hashtable;
            }
            set
            {
                if (_SendHTOfTemp == null)
                    _SendHTOfTemp = new Hashtable();
                _SendHTOfTemp[BP.Web.WebUser.No] = value;
            }
        }
        /// <summary>
        /// 报表属性集合
        /// </summary>
        private static Attrs _AttrsOfRpt = null;
        /// <summary>
        /// 报表属性集合
        /// </summary>
        public static Attrs AttrsOfRpt
        {
            get
            {
                if (_AttrsOfRpt == null)
                {
                    _AttrsOfRpt = new Attrs();
                    _AttrsOfRpt.AddTBInt(GERptAttr.OID, 0, "WorkID", true, true);
                    _AttrsOfRpt.AddTBInt(GERptAttr.FID, 0, "FlowID", false, false);

                    _AttrsOfRpt.AddTBString(GERptAttr.Title, null, "标题", true, false, 0, 10, 10);
                    _AttrsOfRpt.AddTBString(GERptAttr.FlowStarter, null, "发起人", true, false, 0, 10, 10);
                    _AttrsOfRpt.AddTBString(GERptAttr.FlowStartRDT, null, "发起时间", true, false, 0, 10, 10);
                    _AttrsOfRpt.AddTBString(GERptAttr.WFState, null, "状态", true, false, 0, 10, 10);

                    //Attr attr = new Attr();
                    //attr.Desc = "流程状态";
                    //attr.Key = "WFState";
                    //attr.MyFieldType = FieldType.Enum;
                    //attr.UIBindKey = "WFState";
                    //attr.UITag = "@0=进行中@1=已经完成";

                    _AttrsOfRpt.AddDDLSysEnum(GERptAttr.WFState, 0, "流程状态", true, true, GERptAttr.WFState);
                    _AttrsOfRpt.AddTBString(GERptAttr.FlowEmps, null, "参与人", true, false, 0, 10, 10);
                    _AttrsOfRpt.AddTBString(GERptAttr.FlowEnder, null, "结束人", true, false, 0, 10, 10);
                    _AttrsOfRpt.AddTBString(GERptAttr.FlowEnderRDT, null, "结束时间", true, false, 0, 10, 10);
                    _AttrsOfRpt.AddTBDecimal(GERptAttr.FlowEndNode, 0, "结束节点", true, false);
                    _AttrsOfRpt.AddTBDecimal(GERptAttr.FlowDaySpan, 0, "跨度(天)", true, false);
                    //_AttrsOfRpt.AddTBString(GERptAttr.FK_NY, null, "隶属月份", true, false, 0, 10, 10);
                }
                return _AttrsOfRpt;
            }
        }
        #endregion 属性

        #region 其他配置.
       
        /// <summary>
        /// 帮助
        /// </summary>
        /// <param name="id1"></param>
        /// <param name="id2"></param>
        /// <returns></returns>
        public static string GenerHelpCCForm(string text, string id1, string id2)
        {
            if (id1 == null)
                return "<div style='float:right' ><a href='http://ccform.mydoc.io' target=_blank><img src='/WF/Img/Help.gif'>" + text + "</a></div>";
            else
                return "<div style='float:right' ><a href='" + id1 + "' target=_blank><img src='/WF/Img/Help.gif'>" + text + "</a></div>";
        }
        public static string GenerHelpCCFlow(string text, string id1, string id2)
        {
            return "<div style='float:right' ><a href='" + id1 + "' target=_blank><img src='/WF/Img/Help.gif'>" + text + "</a></div>";
        }
        public static string NodeImagePath
        {
            get
            {
                return Glo.IntallPath + "\\Data\\Node\\";
            }
        }
        public static void ClearDBData()
        {
            string sql = "DELETE FROM WF_GenerWorkFlow WHERE fk_flow not in (select no from wf_flow )";
            BP.DA.DBAccess.RunSQL(sql);

            sql = "DELETE FROM WF_GenerWorkerlist WHERE fk_flow not in (select no from wf_flow )";
            BP.DA.DBAccess.RunSQL(sql);
        }
        public static string OEM_Flag = "CCS";
        public static string FlowFileBill
        {
            get { return Glo.IntallPath + "\\DataUser\\Bill\\"; }
        }
        private static string _IntallPath = null;
        public static string IntallPath
        {
            get
            {
                if (_IntallPath == null)
                {
                    if (SystemConfig.IsBSsystem == true)
                        _IntallPath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
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
        private static string _ServerIP = null;
        public static string ServerIP
        {
            get
            {
                if (_ServerIP == null)
                {
                    string ip = "127.0.0.1";
                    System.Net.IPAddress[] addressList = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
                    if (addressList.Length > 1)
                        _ServerIP = addressList[1].ToString();
                    else
                        _ServerIP = addressList[0].ToString();
                }
                return _ServerIP;
            }
            set
            {
                _ServerIP = value;
            }
        }
        /// <summary>
        /// 流程控制器按钮
        /// </summary>
        public static string FlowCtrlBtnPos
        {
            get
            {
                string s = BP.Sys.SystemConfig.AppSettings["FlowCtrlBtnPos"] as string;
                if (s == null || s == "Top")
                    return "Top";
                return "Bottom";
            }
        }
        /// <summary>
        /// 全局的安全验证码
        /// </summary>
        public static string GloSID
        {
            get
            {
                string s = BP.Sys.SystemConfig.AppSettings["GloSID"] as string;
                if (s == null || s == "")
                    s = "sdfq2erre-2342-234sdf23423-323";
                return s;
            }
        }
        /// <summary>
        /// 是否启用检查用户的状态?
        /// 如果启用了:在MyFlow.aspx中每次都会检查当前的用户状态是否被禁
        /// 用，如果禁用了就不能执行任何操作了。启用后，就意味着每次都要
        /// 访问数据库。
        /// </summary>
        public static bool IsEnableCheckUseSta
        {
            get
            {
                string s = BP.Sys.SystemConfig.AppSettings["IsEnableCheckUseSta"] as string;
                if (s == null || s == "0")
                    return false;
                return true;
            }
        }
        /// <summary>
        /// 是否启用显示节点名称
        /// </summary>
        public static bool IsEnableMyNodeName
        {
            get
            {
                string s = BP.Sys.SystemConfig.AppSettings["IsEnableMyNodeName"] as string;
                if (s == null || s == "0")
                    return false;
                return true;
            }
        }
        /// <summary>
        /// 检查一下当前的用户是否仍旧有效使用？
        /// </summary>
        /// <returns></returns>
        public static bool CheckIsEnableWFEmp()
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT UseSta FROM WF_Emp WHERE No=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";
            ps.AddFK_Emp();
            string s = DBAccess.RunSQLReturnStringIsNull(ps, "1");
            if (s == "1" || s == null)
                return true;
            return false;
        }
        /// <summary>
        /// 语言
        /// </summary>
        public static string Language = "CH";
        public static bool IsQL
        {
            get
            {
                string s = BP.Sys.SystemConfig.AppSettings["IsQL"];
                if (s == null || s == "0")
                    return false;
                return true;
            }
        }
        /// <summary>
        /// 是否启用共享任务池？
        /// </summary>
        public static bool IsEnableTaskPool
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyBoolen("IsEnableTaskPool", false);
            }
        }
        /// <summary>
        /// 是否显示标题
        /// </summary>
        public static bool IsShowTitle
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyBoolen("IsShowTitle", false);
            }
        }

        /// <summary>
        /// 用户信息显示格式
        /// </summary>
        public static UserInfoShowModel UserInfoShowModel
        {
            get
            {
                return (UserInfoShowModel)BP.Sys.SystemConfig.GetValByKeyInt("UserInfoShowModel", 0);
            }
        }
        /// <summary>
        /// 产生用户数字签名
        /// </summary>
        /// <returns></returns>
        public static string GenerUserSigantureHtml(string userNo, string userName)
        {
            return "<img src='" + CCFlowAppPath + "DataUser/Siganture/" + userNo + ".jpg' title='" + userName + "' border=0 onerror=\"src='" + CCFlowAppPath + "DataUser/UserICON/DefaultSmaller.png'\" />";
        }
        /// <summary>
        /// 产生用户小图片
        /// </summary>
        /// <returns></returns>
        public static string GenerUserImgSmallerHtml(string userNo, string userName)
        {
            return "<img src='" + CCFlowAppPath + "DataUser/UserICON/" + userNo + "Smaller.png' border=0  style='height:15px;width:15px;padding-right:5px;vertical-align:middle;'  onerror=\"src='" + CCFlowAppPath + "DataUser/UserICON/DefaultSmaller.png'\" />" + userName;
        }
        /// <summary>
        /// 产生用户大图片
        /// </summary>
        /// <returns></returns>
        public static string GenerUserImgHtml(string userNo, string userName)
        {
            return "<img src='" + CCFlowAppPath + "DataUser/UserICON/" + userNo + ".png'  style='padding-right:5px;width:60px;height:80px;border:0px;text-align:middle' onerror=\"src='" + CCFlowAppPath + "DataUser/UserICON/Default.png'\" /><br>" + userName;
        }
        /// <summary>
        /// 更新主表的SQL
        /// </summary>
        public static string UpdataMainDeptSQL
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKey("UpdataMainDeptSQL", "UPDATE Port_Emp SET FK_Dept=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "FK_Dept WHERE No=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "No");
            }
        }
        /// <summary>
        /// 更新SID的SQL
        /// </summary>
        public static string UpdataSID
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKey("UpdataSID", "UPDATE Port_Emp SET SID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "SID WHERE No=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "No");
            }
        }
        /// <summary>
        /// 下载sl的地址
        /// </summary>
        public static string SilverlightDownloadUrl
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKey("SilverlightDownloadUrl", "http://go.microsoft.com/fwlink/?LinkID=124807");
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
                    return "(" + no + ")";
                case UserInfoShowModel.UserIDUserName:
                    return "(" + no + "," + name + ")";
                case UserInfoShowModel.UserNameOnly:
                    return "(" + name + ")";
                default:
                    throw new Exception("@没有判断的格式类型.");
                    break;
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
                string corpid = BP.Sys.SystemConfig.WX_CorpID;
                string corpsecret = BP.Sys.SystemConfig.WX_AppSecret;
                if (string.IsNullOrEmpty(corpid) || string.IsNullOrEmpty(corpsecret))
                    return false;

                return true;
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
                string corpid = BP.Sys.SystemConfig.Ding_CorpID;
                string corpsecret = BP.Sys.SystemConfig.Ding_CorpSecret;
                if (string.IsNullOrEmpty(corpid) || string.IsNullOrEmpty(corpsecret))
                    return false;

                return true;
            }
        }
        /// <summary>
        /// 运行模式
        /// </summary>
        public static OSModel OSModel
        {
            get
            {
                OSModel os = (OSModel)BP.Sys.SystemConfig.GetValByKeyInt("OSModel", 0);
                return os;
            }
        }
        /// <summary>
        /// 是否是集团使用
        /// </summary>
        public static bool IsUnit
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyBoolen("IsUnit", false);
            }
        }
        /// <summary>
        /// 是否启用制度
        /// </summary>
        public static bool IsEnableZhiDu
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyBoolen("IsEnableZhiDu", false);
            }
        }
        /// <summary>
        /// 是否删除流程注册表数据？
        /// </summary>
        public static bool IsDeleteGenerWorkFlow
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyBoolen("IsDeleteGenerWorkFlow", false);
            }
        }
        /// <summary>
        /// 是否检查表单树字段填写是否为空
        /// </summary>
        public static bool IsEnableCheckFrmTreeIsNull
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyBoolen("IsEnableCheckFrmTreeIsNull", true);
            }
        }

        /// <summary>
        /// 是否启动工作时打开新窗口
        /// </summary>
        public static int IsWinOpenStartWork
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyInt("IsWinOpenStartWork", 1);
            }
        }
        /// <summary>
        /// 是否打开待办工作时打开窗口
        /// </summary>
        public static bool IsWinOpenEmpWorks
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyBoolen("IsWinOpenEmpWorks", true);
            }
        }
        /// <summary>
        /// 是否启用消息系统消息。
        /// </summary>
        public static bool IsEnableSysMessage
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyBoolen("IsEnableSysMessage", true);
            }
        }
        /// <summary>
        /// 与ccflow流程服务相关的配置: 执行自动任务节点，间隔的时间，以分钟计算，默认为2分钟。
        /// </summary>
        public static int AutoNodeDTSTimeSpanMinutes
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyInt("AutoNodeDTSTimeSpanMinutes", 2);
            }
        }
        /// <summary>
        /// ccim集成的数据库.
        /// 是为了向ccim写入消息.
        /// </summary>
        public static string CCIMDBName
        {
            get
            {
                string baseUrl = BP.Sys.SystemConfig.AppSettings["CCIMDBName"];
                if (string.IsNullOrEmpty(baseUrl) == true)
                    baseUrl = "ccPort.dbo";
                return baseUrl;
            }
        }
        /// <summary>
        /// 主机
        /// </summary>
        public static string HostURL
        {
            get
            {
                string baseUrl = BP.Sys.SystemConfig.AppSettings["HostURL"];
                

                if (string.IsNullOrEmpty(baseUrl) == true)
                    baseUrl = "http://127.0.0.1/";

                if (baseUrl.Substring(baseUrl.Length - 1) != "/")
                    baseUrl = baseUrl + "/";
                return baseUrl;
            }
        }
        public static string CurrPageID
        {
            get
            {
                try
                {
                    string url = BP.Sys.Glo.Request.RawUrl;

                    int i = url.LastIndexOf("/") + 1;
                    int i2 = url.IndexOf(".aspx") - 6;
                    try
                    {
                        url = url.Substring(i);
                        return url.Substring(0, url.IndexOf(".aspx"));

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message + url + " i=" + i + " i2=" + i2);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("获取当前PageID错误:" + ex.Message);
                }
            }
        }


        //用户表单风格控制
        public static string GetUserStyle
        {
            get
            {
                //BP.WF.Port.WFEmp emp = new Port.WFEmp(WebUser.No);
                //if(string.IsNullOrEmpty(emp.Style) || emp.Style=="0")
                //{
                string userStyle = BP.Sys.SystemConfig.AppSettings["UserStyle"];
                if (string.IsNullOrEmpty(userStyle))
                    return "ccflow默认";
                else
                    return userStyle;
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

            int timeInt= int.Parse(dt.ToString("HHmm"));

            //判断是否在A区间, 如果是，就返回A区间的时间点.
            if (Glo.AMFromInt >=timeInt)
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
        private static DateTime AddMinutes(DateTime dt, int minutes)
        {
            //如果没有设置,就返回.
            if (minutes == 0)
                return dt;

            //设置成工作时间.
            dt = SetToWorkTime(dt);

            //首先判断是否是在一天整的时间完成.
            if (minutes == Glo.AMPMHours*60)
            {
                 /*如果需要在一天完成*/
                dt = DataType.AddDays(dt, 1); 
                return dt;
            }

            //判断是否是AM.
            bool isAM = false;
            int timeInt=int.Parse(dt.ToString("HHmm"));
            if (Glo.AMToInt > timeInt)
                isAM = true;

            #region 如果是当天的情况.
            //如果规定的时间在 1天之内.
            if (minutes/60/ Glo.AMPMHours < 1)
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
                            DateTime mydt = DataType.ParseSysDateTime2DateTime( dt.ToString("yyyy-MM-dd") + " " + Glo.PMTo);
                            return mydt.AddMinutes( minutes-leftMuit);
                        }

                        //说明要跨到第2天上去了.
                        dt = DataType.AddDays(dt, 1);
                        return Glo.AddMinutes(dt.ToString("yyyy-MM-dd") + " " + Glo.AMFrom, minutes-leftMuit );
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
                        int leftMin= minutes - (int)ts.TotalMinutes;

                        /*否则要计算到第2天上去了， 计算时间要从下一个有效的工作日上班时间开始. */
                        dt = DataType.AddDays(DataType.ParseSysDateTime2DateTime(dt.ToString("yyyy-MM-dd") + " " + Glo.AMFrom), 1);

                        //递归调用,让其在次日的上班时间在增加，分钟数。
                        return Glo.AddMinutes(dt, leftMin);
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
            return AddMinutes(dt, minutes);
        }
        /// <summary>
        /// 在指定的日期上增加n天n小时，并考虑节假日
        /// </summary>
        /// <param name="sysdt">指定的日期</param>
        /// <param name="day">天数</param>
        /// <param name="minutes">分钟数</param>
        /// <returns>返回计算后的日期</returns>
        public static DateTime AddDayHoursSpan(string specDT, int day, int minutes)
        {
            DateTime mydt = BP.DA.DataType.AddDays(specDT,day);
            return Glo.AddMinutes(mydt, minutes);
        }
          /// <summary>
        /// 在指定的日期上增加n天n小时，并考虑节假日
        /// </summary>
        /// <param name="sysdt">指定的日期</param>
        /// <param name="day">天数</param>
        /// <param name="minutes">分钟数</param>
        /// <returns>返回计算后的日期</returns>
        public static DateTime AddDayHoursSpan(DateTime specDT, int day, int minutes)
        {
            DateTime mydt = BP.DA.DataType.AddDays(specDT, day);
            return Glo.AddMinutes(mydt, minutes);
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
        public static void InitCH(Flow fl, Node nd, Int64 workid, Int64 fid, string title)
        {
            InitCH(fl, nd, workid, fid, title, null, null, DateTime.Now);
        }
        /// <summary>
        /// 当流程发送下去以后，就开始执行考核。
        /// </summary>
        /// <param name="fl">流程</param>
        /// <param name="nd">节点</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">FID</param>
        /// <param name="title">标题</param>
        /// <param name="dtNow">计算的当前时间，如果为null,就取当前日期.</param>
        public static void InitCH(Flow fl, Node nd, Int64 workid, Int64 fid, string title, string prvRDT,string sdt, DateTime dtNow)
        {
            //开始节点不考核.
            if (nd.IsStartNode)
                return;

            //如果设置为0 则不考核.
            if (nd.TSpanDay == 0 && nd.TSpanHour==0)
                return;

            if (dtNow == null)
                dtNow = DateTime.Now;

            if (sdt == null || prvRDT == null)
            {
                string dbstr = SystemConfig.AppCenterDBVarStr;
                Paras ps = new Paras();
                switch (SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                        ps.SQL = "SELECT TOP 1 RDT,SDT FROM WF_GENERWORKERLIST  WHERE WorkID=" + dbstr + "WorkID AND FK_Emp=" + dbstr + "FK_Emp AND FK_Node=" + dbstr + "FK_Node ORDER BY RDT DESC";
                        break;
                    case DBType.Oracle:
                        ps.SQL = "SELECT  RDT,SDT FROM WF_GENERWORKERLIST  WHERE WorkID=" + dbstr + "WorkID AND FK_Emp=" + dbstr + "FK_Emp AND FK_Node=" + dbstr + "FK_Node AND ROWNUM=1 ORDER BY RDT DESC ";
                        break;
                    case DBType.MySQL:
                        ps.SQL = "SELECT  RDT,SDT FROM WF_GENERWORKERLIST  WHERE WorkID=" + dbstr + "WorkID AND FK_Emp=" + dbstr + "FK_Emp AND FK_Node=" + dbstr + "FK_Node ORDER BY RDT DESC limit 0,1 ";
                        break;
                    default:
                        break;
                }
                ps.Add("WorkID", workid);
                ps.Add("FK_Emp", WebUser.No);
                ps.Add("FK_Node", nd.NodeID);

                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count == 0)
                    return;

                prvRDT = dt.Rows[0][0].ToString();
                sdt = dt.Rows[0][1].ToString(); 
            }

            #region 初始化基础数据.
            BP.WF.Data.CH ch = new CH();
            ch.WorkID = workid;
            ch.FID = fid;
            ch.Title = title;

            int hh = (int)nd.TSpanHour;
            float mm = (nd.TSpanHour - hh)*60;
            ch.TSpan = nd.TSpanDay + "天" + hh + "时" + mm + "分";
            ch.FK_NY = dtNow.ToString("yyyy-MM");

            ch.DTFrom = prvRDT;  //任务下达时间.
            ch.SDT = sdt; //应该完成时间.

            ch.FK_Flow = nd.FK_Flow;
            ch.FK_FlowT = nd.FlowName;

            ch.FK_Node = nd.NodeID;
            ch.FK_NodeT = nd.Name;

            ch.FK_Dept = WebUser.FK_Dept;
            ch.FK_DeptT = WebUser.FK_DeptName;

            ch.FK_Emp = WebUser.No;
            ch.FK_EmpT = WebUser.Name;

            //求出是第几个周.
           // ch.Week = (int)dtNow.w;
            System.Globalization.CultureInfo myCI =  
                        new System.Globalization.CultureInfo("zh-CN");            
            ch.WeekNum  = myCI.Calendar.GetWeekOfYear(dtNow,System.Globalization.CalendarWeekRule.FirstDay,System.DayOfWeek.Monday);

            // mypk.
            ch.MyPK = nd.NodeID + "_" + workid + "_" + fid + "_" + WebUser.No;
            #endregion 初始化基础数据.


           // 求出结算时间点 dtFrom.
            DateTime dtFrom = BP.DA.DataType.ParseSysDateTime2DateTime(ch.DTFrom);
            dtFrom = Glo.SetToWorkTime(dtFrom);

            //当前时间.  -设置结算时间到.
            ch.DTTo = dtNow.ToString(DataType.SysDataTimeFormat); // dtto..
            dtNow = Glo.SetToWorkTime(dtNow);

            int dtHHmm = 0;
            if (dtFrom.Year == dtNow.Year && dtFrom.Month == dtNow.Month && dtFrom.Day == dtFrom.Day)
            {
                // 计算发送时间是否是中午?
                dtHHmm = int.Parse(dtFrom.ToString("HHmm"));
                bool isSendAM = false;
                if (dtHHmm >= Glo.AMFromInt && dtHHmm <= Glo.AMToInt)
                {
                    /*发送人发送时间是上午, 并且处理人处理时间也是上午.*/
                    isSendAM = true;
                }

                // 计算处理时间是否是中午？
                dtHHmm = int.Parse(dtFrom.ToString("HHmm"));
                bool isCurrAM = false;
                if (dtHHmm >= Glo.AMFromInt && dtHHmm <= Glo.AMToInt)
                {
                    /*发送人发送时间是上午, 并且处理人处理时间也是上午.*/
                    isCurrAM = true;
                }

                /* 如果是同一天.  如果都是中午.*/
                if (isSendAM && isCurrAM)
                {
                    TimeSpan ts = dtNow - dtFrom;
                    ch.UseMinutes += (int)ts.TotalMinutes; // 得到实际用的时间.
                }

                /* 如果是同一天.  如果都是下午.*/
                if (isSendAM == false && isCurrAM == false)
                {
                    TimeSpan ts = dtNow - dtFrom;

                    //两个时间差，并减去午休的时间.
                    ch.UseMinutes += (int)ts.TotalMinutes; // 得到实际用的时间.
                }

                /* 如果是同一天.  如果一个是中午一个是下午.*/
                if (isSendAM == true && isCurrAM == false)
                {
                    float f60 = 60;
                    TimeSpan ts = dtNow - dtFrom;
                    //ts.TotalMinutes
                    float hours = (float)ts.TotalHours - Glo.AMPMTimeSpan; // 得到实际用的时间.

                    // 实际使用时间.
                    ch.UseMinutes += (int)hours*60;
                }

                //求超过时限.
                ch.OverMinutes = ch.UseMinutes - nd.TSpanMinues;

                //设置时限状态.
                if (ch.OverMinutes > 0)
                {   
                    /* 如果是正数，就说明它是一个超期完成的状态. */
                    if (ch.OverMinutes / 2 > nd.TSpanMinues)
                        ch.CHSta = CHSta.ChaoQi; //如果超过了时间的一半，就是超期.
                    else
                        ch.CHSta = CHSta.YuQi;   //在规定的时间段以内完成，就是预期。
                }
                else
                {
                    /* 是负数就是提前完成. */
                    if (ch.OverMinutes / 2 > -nd.TSpanMinues)
                        ch.CHSta = CHSta.JiShi; //如果提前了一半的时间，就是及时完成.
                    else
                        ch.CHSta = CHSta.AnQi; // 否则就是按期完成.
                }

                #region 计算出来可以识别的分钟数.
                //求使用天数.
                float day = ch.UseMinutes / 60f / Glo.AMPMHours;
                int dayInt = (int)day;

                //求小时数.
                float hour = (ch.UseMinutes - dayInt * Glo.AMPMHours*60f)/60f;
                int hourInt = (int)hour;

                //求分钟数.
                float minute = (hour - hourInt)*60;

                //使用时间.
                ch.UseTime = dayInt + "天" + hourInt + "时" + minute + "分";


                //求预期天数.
                int overMinus = Math.Abs(ch.OverMinutes);
                day = overMinus / 60f / Glo.AMPMHours;
                  dayInt = (int)day;

                //求小时数.
                  hour = (overMinus - dayInt * Glo.AMPMHours * 60f) / 60f;
                  hourInt = (int)hour;

                //求分钟数.
                  minute = (hour - hourInt) * 60;

                //使用时间.
                if (ch.OverMinutes >0)
                    ch.OverTime = "预期:" + dayInt + "天" + hourInt + "小时" + (int)minute + "分";
                else
                    ch.OverTime = "提前:" + dayInt + "天" + hourInt + "小时" + (int)minute + "分";

                #endregion 计算出来可以识别的分钟数.

                  //执行保存.
                try
                {
                    ch.DirectInsert();
                }
                catch
                {
                    ch.CheckPhysicsTable();
                    ch.DirectUpdate();
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
                return BP.Sys.SystemConfig.GetValByKey("AMFrom", "08:30");
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
                return BP.Sys.SystemConfig.GetValByKeyFloat("AMPMHours", 8);
            }
        }
        /// <summary>
        /// 中午间隔的小时数
        /// </summary>
        public static float AMPMTimeSpan
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKeyFloat("AMPMTimeSpan", 1);
            }
        }
        /// <summary>
        /// 中午时间到
        /// </summary>
        public static string AMTo
        {
            get
            {
                return BP.Sys.SystemConfig.GetValByKey("AMTo", "11:30");
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
                return BP.Sys.SystemConfig.GetValByKey("PMFrom", "13:30");
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
                return BP.Sys.SystemConfig.GetValByKey("PMTo", "17:30");
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
        /// 获得一个表单的动态权限字段
        /// </summary>
        /// <param name="exts"></param>
        /// <param name="nd"></param>
        /// <param name="en"></param>
        /// <param name="md"></param>
        /// <param name="attrs"></param>
        /// <returns></returns>
        public static string GenerActiveFiels(MapExts exts, Node nd, Entity en, MapData md, MapAttrs attrs)
        {
            string strs = "";
            foreach (MapExt me in exts)
            {
                if (me.ExtType != MapExtXmlList.SepcFiledsSepcUsers)
                    continue;
                bool isCando = false;
                if (me.Tag1 != "")
                {
                    string tag1 = me.Tag1 + ",";
                    if (tag1.Contains(BP.Web.WebUser.No + ","))
                    {
                        //根据设置的人员计算.
                        isCando = true;
                    }
                }

                if (me.Tag2 != "")
                {
                    //根据sql判断.
                    string sql = me.Tag2.Clone() as string;
                    sql = BP.WF.Glo.DealExp(sql, en, null);
                    if (BP.DA.DBAccess.RunSQLReturnValFloat(sql) > 0)
                        isCando = true;
                }

                if (me.Tag3 != "" && BP.Web.WebUser.FK_Dept == me.Tag3)
                {
                    //根据部门编号判断.
                    isCando = true;
                }

                if (isCando == false)
                    continue;
                strs += me.Doc;
            }
            return strs;
        }
        /// <summary>
        /// 转到消息显示界面.
        /// </summary>
        /// <param name="info"></param>
        public static void ToMsg(string info)
        {
            //string rowUrl = BP.Sys.Glo.Request.RawUrl;
            //if (rowUrl.Contains("&IsClient=1"))
            //{
            //    /*说明这是vsto调用的.*/
            //    return;
            //}

            System.Web.HttpContext.Current.Session["info"] = info;
            System.Web.HttpContext.Current.Response.Redirect(Glo.CCFlowAppPath + "WF/MyFlowInfo.aspx?Msg=" + DataType.CurrentDataTimess, false);
        }
        public static void ToMsgErr(string info)
        {
            info = "<font color=red>" + info + "</font>";
            System.Web.HttpContext.Current.Session["info"] = info;
            System.Web.HttpContext.Current.Response.Redirect(Glo.CCFlowAppPath + "WF/MyFlowInfo.aspx?Msg=" + DataType.CurrentDataTimess, false);
        }
        /// <summary>
        /// 检查流程发起限制
        /// </summary>
        /// <param name="flow">流程</param>
        /// <param name="wk">开始节点工作</param>
        /// <returns></returns>
        public static bool CheckIsCanStartFlow_InitStartFlow(Flow flow)
        {
            StartLimitRole role = flow.StartLimitRole;
            if (role == StartLimitRole.None)
                return true;

            string sql = "";
            string ptable = flow.PTable;

            #region 按照时间的必须是，在表单加载后判断, 不管用户设置是否正确.
            DateTime dtNow = DateTime.Now;
            if (role == StartLimitRole.Day)
            {
                /* 仅允许一天发起一次 */
                sql = "SELECT COUNT(*) as Num FROM " + ptable + " WHERE RDT LIKE '" + DataType.CurrentData + "%' AND WFState NOT IN(0,1) AND FlowStarter='" + WebUser.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                {
                    if (flow.StartLimitPara == "")
                        return true;

                    //判断时间是否在设置的发起范围内. 配置的格式为 @11:00-12:00@15:00-13:45
                    string[] strs = flow.StartLimitPara.Split('@');
                    foreach (string str in strs)
                    {
                        if (string.IsNullOrEmpty(str))
                            continue;
                        string[] timeStrs = str.Split('-');
                        string tFrom = DateTime.Now.ToString("yyyy-MM-dd") + " " + timeStrs[0].Trim();
                        string tTo = DateTime.Now.ToString("yyyy-MM-dd") + " " + timeStrs[1].Trim();
                        if (DataType.ParseSysDateTime2DateTime(tFrom) <= dtNow && dtNow >= DataType.ParseSysDateTime2DateTime(tTo))
                            return true;
                    }
                    return false;
                }
                else
                    return false;
            }

            if (role == StartLimitRole.Week)
            {
                /*
                 * 1, 找出周1 与周日分别是第几日.
                 * 2, 按照这个范围去查询,如果查询到结果，就说明已经启动了。
                 */
                sql = "SELECT COUNT(*) as Num FROM " + ptable + " WHERE RDT >= '" + DataType.WeekOfMonday(dtNow) + "' AND WFState NOT IN(0,1) AND FlowStarter='" + WebUser.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                {
                    if (flow.StartLimitPara == "")
                        return true; /*如果没有时间的限制.*/

                    //判断时间是否在设置的发起范围内. 
                    // 配置的格式为 @Sunday,11:00-12:00@Monday,15:00-13:45, 意思是.周日，周一的指定的时间点范围内可以启动流程.

                    string[] strs = flow.StartLimitPara.Split('@');
                    foreach (string str in strs)
                    {
                        if (string.IsNullOrEmpty(str))
                            continue;

                        string weekStr = DateTime.Now.DayOfWeek.ToString().ToLower();
                        if (str.ToLower().Contains(weekStr) == false)
                            continue; // 判断是否当前的周.

                        string[] timeStrs = str.Split(',');
                        string tFrom = DateTime.Now.ToString("yyyy-MM-dd") + " " + timeStrs[0].Trim();
                        string tTo = DateTime.Now.ToString("yyyy-MM-dd") + " " + timeStrs[1].Trim();
                        if (DataType.ParseSysDateTime2DateTime(tFrom) <= dtNow && dtNow >= DataType.ParseSysDateTime2DateTime(tTo))
                            return true;
                    }
                    return false;
                }
                else
                    return false;
            }

            // #warning 没有考虑到周的如何处理.

            if (role == StartLimitRole.Month)
            {
                sql = "SELECT COUNT(*) as Num FROM " + ptable + " WHERE FK_NY = '" + DataType.CurrentYearMonth + "' AND WFState NOT IN(0,1) AND FlowStarter='" + WebUser.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                {
                    if (flow.StartLimitPara == "")
                        return true;

                    //判断时间是否在设置的发起范围内. 配置格式: @-01 12:00-13:11@-15 12:00-13:11 , 意思是：在每月的1号,15号 12:00-13:11可以启动流程.
                    string[] strs = flow.StartLimitPara.Split('@');
                    foreach (string str in strs)
                    {
                        if (string.IsNullOrEmpty(str))
                            continue;
                        string[] timeStrs = str.Split('-');
                        string tFrom = DateTime.Now.ToString("yyyy-MM-") + " " + timeStrs[0].Trim();
                        string tTo = DateTime.Now.ToString("yyyy-MM-") + " " + timeStrs[1].Trim();
                        if (DataType.ParseSysDateTime2DateTime(tFrom) <= dtNow && dtNow >= DataType.ParseSysDateTime2DateTime(tTo))
                            return true;
                    }
                    return false;
                }
                else
                    return false;
            }

            if (role == StartLimitRole.JD)
            {
                sql = "SELECT COUNT(*) as Num FROM " + ptable + " WHERE FK_NY = '" + DataType.CurrentAPOfJD + "' AND WFState NOT IN(0,1) AND FlowStarter='" + WebUser.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                {
                    if (flow.StartLimitPara == "")
                        return true;

                    //判断时间是否在设置的发起范围内.
                    string[] strs = flow.StartLimitPara.Split('@');
                    foreach (string str in strs)
                    {
                        if (string.IsNullOrEmpty(str))
                            continue;
                        string[] timeStrs = str.Split('-');
                        string tFrom = DateTime.Now.ToString("yyyy-") + " " + timeStrs[0].Trim();
                        string tTo = DateTime.Now.ToString("yyyy-") + " " + timeStrs[1].Trim();
                        if (DataType.ParseSysDateTime2DateTime(tFrom) <= dtNow && dtNow >= DataType.ParseSysDateTime2DateTime(tTo))
                            return true;
                    }
                    return false;
                }
                else
                    return false;
            }

            if (role == StartLimitRole.Year)
            {
                sql = "SELECT COUNT(*) as Num FROM " + ptable + " WHERE FK_NY LIKE '" + DataType.CurrentYear + "%' AND WFState NOT IN(0,1) AND FlowStarter='" + WebUser.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                {
                    if (flow.StartLimitPara == "")
                        return true;

                    //判断时间是否在设置的发起范围内.
                    string[] strs = flow.StartLimitPara.Split('@');
                    foreach (string str in strs)
                    {
                        if (string.IsNullOrEmpty(str))
                            continue;
                        string[] timeStrs = str.Split('-');
                        string tFrom = DateTime.Now.ToString("yyyy-") + " " + timeStrs[0].Trim();
                        string tTo = DateTime.Now.ToString("yyyy-") + " " + timeStrs[1].Trim();
                        if (DataType.ParseSysDateTime2DateTime(tFrom) <= dtNow && dtNow >= DataType.ParseSysDateTime2DateTime(tTo))
                            return true;
                    }
                    return false;
                }
                else
                    return false;
            }
            #endregion 按照时间的必须是，在表单加载后判断, 不管用户设置是否正确.

            return true;
        }
        /// <summary>
        /// 当要发送是检查流程是否可以允许发起.
        /// </summary>
        /// <param name="flow">流程</param>
        /// <param name="wk">开始节点工作</param>
        /// <returns></returns>
        public static bool CheckIsCanStartFlow_SendStartFlow(Flow flow, Work wk)
        {
            StartLimitRole role = flow.StartLimitRole;
            if (role == StartLimitRole.None)
                return true;

            string sql = "";
            string ptable = flow.PTable;

            if (role == StartLimitRole.ColNotExit)
            {
                /* 指定的列名集合不存在，才可以发起流程。*/

                //求出原来的值.
                string[] strs = flow.StartLimitPara.Split(',');
                string val = "";
                foreach (string str in strs)
                {
                    if (string.IsNullOrEmpty(str) == true)
                        continue;
                    try
                    {
                        val += wk.GetValStringByKey(str);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("@流程设计错误,您配置的检查参数(" + flow.StartLimitPara + "),中的列(" + str + ")已经不存在表单里.");
                    }
                }

                //找出已经发起的全部流程.
                sql = "SELECT " + flow.StartLimitPara + " FROM " + ptable + " WHERE  WFState NOT IN(0,1) AND FlowStarter='" + WebUser.No + "'";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    string v = dr[0] + "" + dr[1] + "" + dr[2];
                    if (v == val)
                        return false;
                }
                return true;
            }

            // 配置的sql,执行后,返回结果是 0 .
            if (role == StartLimitRole.ResultIsZero)
            {
                sql = BP.WF.Glo.DealExp(flow.StartLimitPara, wk, null);
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                    return true;
                else
                    return false;
            }

            // 配置的sql,执行后,返回结果是 <> 0 .
            if (role == StartLimitRole.ResultIsNotZero)
            {
                sql = BP.WF.Glo.DealExp(flow.StartLimitPara, wk, null);
                if (DBAccess.RunSQLReturnValInt(sql, 0) != 0)
                    return true;
                else
                    return false;
            }
            return true;
        }
        #endregion 其他方法。


        
    }
}
