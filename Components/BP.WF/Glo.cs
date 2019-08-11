using System;
using System.Drawing;
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
        public static string GenerGanttDataOfSubFlows(Int64 workID)
        {
            GenerWorkFlow gwf = new GenerWorkFlow(workID);

            //增加子流程数据.
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve("PWorkID", workID);
            string json = "[";


            //主流程的计划完成日期，与实际完成日期的两个时间段.
            json += " { id:'001', name:'总体计划', ";
            json += "series:[{ ";
            json += " name: '项目计划', ";
            json += " start:  " + ToData(gwf.RDT) + ", ";
            json += " end: " + ToData(gwf.SDTOfFlow) + ",";
            json += " TodoSta: " + gwf.TodoSta + ", ";
            json += " color: '#f0f0f0' ";
            json += "},";

            json += "{ name: '实际执行', ";
            json += " start:  " + ToData(gwf.RDT) + ",";
            json += " end: " + ToData(gwf.SendDT) + ",";
            json += " TodoSta: " + gwf.TodoSta + ",";
            json += " color: '#f0f0f0' ";
            json += "}]";
            json += "},";


            //获得节点.
            Nodes nds = new Nodes(gwf.FK_Flow);
            nds.Retrieve("FK_Flow", gwf.FK_Flow, "Step");

            SubFlows subs = new SubFlows();
            subs.Retrieve(SubFlowAttr.FK_Flow, gwf.FK_Flow);

            int idxNode = 0;
            foreach (Node nd in nds)
            {
                idxNode++;

                //里程碑.
                json += " { id:'" + nd.NodeID + "', name:'" + nd.Name + "', ";

                string series = "";
                foreach (SubFlow sub in subs)
                {
                    if (sub.FK_Node != nd.NodeID)
                        continue;

                    //增加子流成.
                    int idx = 0;
                    string dtlsSubFlow = "";
                    foreach (GenerWorkFlow subGWF in gwfs)
                    {
                        if (subGWF.FK_Flow != sub.SubFlowNo)
                            continue;

                        dtlsSubFlow += "{ ";
                        dtlsSubFlow += " name: '" + subGWF.FlowName + "(计划)',";
                        dtlsSubFlow += " start:  " + ToData(gwf.RDT) + ",";
                        dtlsSubFlow += " end: " + ToData(gwf.SDTOfFlow) + ",";
                        dtlsSubFlow += " TodoSta: -2, ";
                        dtlsSubFlow += " color: 'brue' ";
                        dtlsSubFlow += "},";

                        dtlsSubFlow += "{ ";
                        dtlsSubFlow += " name: '(实际)',";
                        dtlsSubFlow += " start:  " + ToData(gwf.RDT) + ",";
                        dtlsSubFlow += " end: " + ToData(gwf.SendDT) + ",";
                        dtlsSubFlow += " TodoSta: " + gwf.TodoSta + ", ";
                        dtlsSubFlow += " color: '#f0f0f0' ";
                        dtlsSubFlow += "},";

                    }

                    if (DataType.IsNullOrEmpty(dtlsSubFlow) == false)
                        dtlsSubFlow = dtlsSubFlow.Substring(0, dtlsSubFlow.Length - 1);

                    //如果没有启动子流程，就需要显示空白的。
                    if (DataType.IsNullOrEmpty(dtlsSubFlow) == true)
                    {
                        dtlsSubFlow += "{ ";
                        dtlsSubFlow += " name: '" + sub.SubFlowNo + " - " + sub.SubFlowName + "', ";
                        dtlsSubFlow += " start:  " + ToData(DataType.CurrentData) + ", ";
                        dtlsSubFlow += " end:  " + ToData(DataType.CurrentData) + ", ";
                        dtlsSubFlow += " TodoSta: -1, ";
                        dtlsSubFlow += " color: 'blue' ";
                        dtlsSubFlow += "}";
                    }

                    //从表.
                    series += dtlsSubFlow +"," ;
                     
                }

                if (DataType.IsNullOrEmpty(series) == false)
                    series = series.Substring(0, series.Length - 1);


                if (DataType.IsNullOrEmpty(series))
                    json += " series:[]";
                else
                {
                    json += " series:["+ series + "]";
                }

             
                if (idxNode == nds.Count)
                    json += "}";
                else
                    json += "},";
            }

            json += "]";

            return json;
        }

        /// <summary>
        /// 生成甘特图
        /// </summary>
        /// <param name="workID"></param>
        /// <returns></returns>
        public static string GenerGanttDataOfSubFlowsV20(Int64 workID)
        {
            GenerWorkFlow gwf = new GenerWorkFlow(workID);
            //增加子流程数据.
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve("PWorkID", workID);

            string json = "[";

            json += " { id:'" + gwf.FK_Flow + "', name:'" + gwf.FlowName + "',";

            json += " series:[";
            json += "{ name: \"计划时间\", start:  "+ ToData(gwf.SDTOfFlow)+ ", end: " + ToData(gwf.SDTOfFlow) + ", color: \"#f0f0f0\" },";
            json += "{ name: \"实际工作时间\", start: "+ ToData(gwf.RDT)+ ", end: " + ToData(gwf.SendDT) + " , color: \"#f0f0f0\" }";
            json += "]";

            if (gwfs.Count == 0)
            {
                json += "}";
                json += "]";
                return json;
            }else
            {
                json += "},";
            }

            //增加子流成.
            int idx = 0;
            foreach (GenerWorkFlow subGWF in gwfs)
            {
                idx++;

                json += " { id:'" + subGWF.FK_Flow + "', name:'" + subGWF.FlowName + "',";

                json += " series:[";
                json += "{ name: \"实际工作时间\", start:  " + ToData( gwf.RDT )+ ", end: " + ToData(gwf.SendDT) + " }";
                json += "]";

                if (idx==gwfs.Count)
                {
                    json += "}";
                    json += "]";
                    return json;
                }
                else
                {
                    json += "},";
                }                 
            }

            json += "]";

            return json;             
        }

        public static string ToData(string dtStr)
        {

            DateTime dt = BP.DA.DataType.ParseSysDate2DateTime(dtStr);

            return  "'"+dt.ToString("yyyy-MM-dd")+"'";
             
        }
        /// <summary>
        /// 生成甘特图
        /// </summary>
        /// <returns></returns>
        public static string GenerGanttDataOfSubFlowsV1(Int64 workID)
        {
            //定义解构.
            DataTable dtFlows = new DataTable();
            dtFlows.Columns.Add("id");
            dtFlows.Columns.Add("name");

            DataTable dtSeries = new DataTable();
            dtSeries.TableName = "series";
            dtSeries.Columns.Add("name");
            dtSeries.Columns.Add("start");
            dtSeries.Columns.Add("end");
            dtSeries.Columns.Add("color");
            dtSeries.Columns.Add("RefPK");

            //增加主流程数据.
            GenerWorkFlow gwf = new GenerWorkFlow(workID);
            DataRow dr = dtFlows.NewRow();
            dr["id"] = gwf.FK_Flow;
            dr["name"] = gwf.FlowName;
            dtFlows.Rows.Add(dr);

            DataRow drItem = dtSeries.NewRow();
            drItem["name"] = "项目计划日期";
            drItem["start"] = gwf.RDT;
            drItem["end"] = gwf.SDTOfFlow;
            drItem["color"] = "#f0f0f0";
            drItem["RefPK"] = gwf.FK_Flow;
            dtSeries.Rows.Add(drItem);

            drItem = dtSeries.NewRow();
            drItem["name"] = "项目启动日期";
            drItem["start"] = gwf.RDT;
            drItem["end"] = gwf.SDTOfFlow;
            drItem["color"] = "#f0f0f0";
            drItem["RefPK"] = gwf.FK_Flow;
            dtSeries.Rows.Add(drItem);

             
            //增加子流程数据.
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve("PWorkID", workID);

            foreach (GenerWorkFlow subFlow in gwfs)
            {
                dr = dtFlows.NewRow();
                dr["id"] = subFlow.FK_Flow;
                dr["name"] = subFlow.FlowName;
                dtFlows.Rows.Add(dr);
                 

                drItem = dtSeries.NewRow();
                drItem["name"] = "启动日期";
                drItem["start"] = subFlow.RDT;
                drItem["end"] = subFlow.SDTOfFlow;
                drItem["color"] = "#f0f0f0";
                drItem["RefPK"] = subFlow.FK_Flow;
                dtSeries.Rows.Add(drItem);
            }

            DataSet ds = new DataSet();
            ds.Tables.Add(dtFlows);
            ds.Tables.Add(dtSeries);

            return ToJsonOfGantt(ds);
        }
        public static string ToJsonOfGantt(DataSet ds)
        {
            string json = "[";

            DataTable dtFlows = ds.Tables[0];
            DataTable dtSeries = ds.Tables[1];

           


            json += "]";

            return "";

        }

        #region 多语言处理.
        private static Hashtable _Multilingual_Cache = null;
        public static DataTable getMultilingual_DT(string className)
        {
            if (_Multilingual_Cache == null)
                _Multilingual_Cache = new Hashtable();

            if (_Multilingual_Cache.ContainsKey(className) == false)
            {
                DataSet ds = BP.DA.DataType.CXmlFileToDataSet(BP.Sys.SystemConfig.PathOfData + "\\lang\\xml\\" + className + ".xml");
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
            int num = 0;
            if (p0 == null)
                num = 0;
            if (p1 == null)
                num = 1;
            if (p2 == null)
                num = 2;
            if (p3 == null)
                num = 3;

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
                string s = BP.Sys.SystemConfig.AppSettings["PrintBackgroundWord"];
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
            if (Glo.Platform == Platform.CCFlow)
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
        /// 当前版本号-为了升级使用.
        /// </summary>
        public static int Ver = 20190702;
        /// <summary>
        /// 执行升级
        /// </summary>
        /// <returns></returns>
        public static string UpdataCCFlowVer()
        {
            #region 检查是否需要升级，并更新升级的业务逻辑.
            string updataNote = "";

            /*
             * 升级版本记录:
             * 20150330: 优化发起列表的效率, by:zhoupeng.
             * 2, 升级表单树,支持动态表单树.
             * 1, 执行一次Sender发送人的升级，原来由GenerWorkerList 转入WF_GenerWorkFlow.
             * 0, 静默升级启用日期.2014-12
             */

            if (BP.DA.DBAccess.IsExitsObject("Sys_Serial") == false)
                return "";

            //检查子流程表.
            if (BP.DA.DBAccess.IsExitsObject("WF_NodeSubFlow") == true)
            {
                if (BP.DA.DBAccess.IsExitsTableCol("WF_NodeSubFlow", "OID") == true)
                {
                    DBAccess.RunSQL("DROP TABLE WF_NodeSubFlow");
                    SubFlowHand sub = new SubFlowHand();
                    sub.CheckPhysicsTable();
                }
            }

            //检查表.
            BP.Sys.GloVar gv = new GloVar();
            gv.CheckPhysicsTable();

            //检查表.
            BP.Sys.EnCfg cfg = new BP.Sys.EnCfg();
            cfg.CheckPhysicsTable();

            //检查表.
            BP.Sys.FrmTree frmTree = new BP.Sys.FrmTree();
            frmTree.CheckPhysicsTable();

            BP.Sys.SFTable sf = new SFTable();
            sf.CheckPhysicsTable();

            BP.WF.Template.FrmSubFlow sb = new FrmSubFlow();
            sb.CheckPhysicsTable();


            BP.WF.Template.PushMsg pm = new PushMsg();
            pm.CheckPhysicsTable();

            //修复数据表.
            BP.Sys.GroupField gf = new GroupField();
            gf.CheckPhysicsTable();

            //先升级脚本,就是说该文件如果被修改了就会自动升级.
            UpdataCCFlowVerSQLScript();

            //判断数据库的版本.
            string sql = "SELECT IntVal FROM Sys_Serial WHERE CfgKey='Ver'";
            int currDBVer = DBAccess.RunSQLReturnValInt(sql, 0);
            if (currDBVer != null && currDBVer != 0 && currDBVer >= Ver)
                return null; //不需要升级.

            // 升级fromjson .//NOTE:此处有何用？而且md变量在下方已经声明，编译都通不过，2017-05-20，liuxc
            //MapData md = new MapData();
            //md.FormJson = "";
            #endregion 检查是否需要升级，并更新升级的业务逻辑.

            #region 枚举值
            SysEnumMains enumMains = new SysEnumMains();
            enumMains.RetrieveAll();
            foreach(SysEnumMain enumMain in enumMains)
            {
                SysEnums ens = new SysEnums();
                ens.Delete(SysEnumAttr.EnumKey, enumMain.No);

                //保存数据
                string cfgVal = enumMain.CfgVal;
                string[] strs = cfgVal.Split('@');
                foreach (string s in strs)
                {
                    if (s == "" || s == null)
                        continue;

                    string[] vk = s.Split('=');
                    SysEnum se = new SysEnum();
                    se.IntKey = int.Parse(vk[0]);
                    string[] kvsValues = new string[vk.Length - 1];
                    for (int i = 0; i < kvsValues.Length; i++)
                    {
                        kvsValues[i] = vk[i + 1];
                    }
                    se.Lab = string.Join("=", kvsValues);
                    se.EnumKey = enumMain.No;
                    se.Lang = BP.Web.WebUser.SysLang;
                    se.Insert();
                }
            }
            #endregion

            #region 升级填充数据.
            //pop自动填充
            MapExts exts = new MapExts();
            QueryObject qo = new QueryObject(exts);
            qo.AddWhere(MapExtAttr.ExtType, " LIKE ", "Pop%");
            qo.DoQuery();
            foreach (MapExt ext in exts)
            {
                string mypk = ext.FK_MapData + "_" + ext.AttrOfOper;
                MapAttr ma = new MapAttr();
                ma.MyPK = mypk;
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
                extP.MyPK = ext.MyPK + "_FullData";
                int i = extP.RetrieveFromDBSources();
                if (i == 1)
                    continue;

                extP.ExtType = "FullData";
                extP.FK_MapData = ext.FK_MapData;
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
                string mypk = ext.FK_MapData + "_" + ext.AttrOfOper;
                MapAttr ma = new MapAttr();
                ma.MyPK = mypk;
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
                extP.MyPK = ext.MyPK + "_FullData";
                int i = extP.RetrieveFromDBSources();
                if (i == 1)
                    continue;

                extP.ExtType = "FullData";
                extP.FK_MapData = ext.FK_MapData;
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
                string mypk = ext.FK_MapData + "_" + ext.AttrOfOper;
                MapAttr ma = new MapAttr();
                ma.MyPK = mypk;
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
                extP.MyPK = ext.MyPK + "_FullData";
                int i = extP.RetrieveFromDBSources();
                if (i == 1)
                    continue;

                extP.ExtType = "FullData";
                extP.FK_MapData = ext.FK_MapData;
                extP.AttrOfOper = ext.AttrOfOper;
                extP.DBType = ext.DBType;
                extP.Doc = ext.Doc;


                //填充从表
                extP.Tag1 = ext.Tag1;
                //填充下拉框
                extP.Tag = ext.Tag;

                extP.Insert(); //执行插入.

            }
            #endregion 升级 填充数据.

            string msg = "";
            try
            {

                #region 创建缺少的视图 Port_Inc.  @fanleiwei 需要翻译.
                if (DBAccess.IsExitsObject("Port_Inc") == false)
                {
                    sql = "CREATE VIEW Port_Inc AS SELECT * FROM Port_Dept WHERE (No='100' OR No='1060' OR No='1070') ";
                    DBAccess.RunSQL(sql);
                }
                #endregion 创建缺少的视图 Port_Inc.

                #region 升级事件.
                if (DBAccess.IsExitsTableCol("Sys_FrmEvent", "DoType") == true)
                {
                    BP.Sys.FrmEvent fe = new FrmEvent();
                    fe.CheckPhysicsTable();

                    DBAccess.RunSQL("UPDATE Sys_FrmEvent SET EventDoType=DoType  ");
                    DBAccess.RunSQL("ALTER TABLE Sys_FrmEvent   DROP COLUMN	DoType  ");
                }
                #endregion

                #region 修复丢失的发起人.
                Flows fls = new Flows();
                fls.RetrieveAll();
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
                BP.WF.Port.WFEmp wfemp = new Port.WFEmp();
                wfemp.CheckPhysicsTable();

                #region 更新wf_emp. 的字段类型. 2019.06.19
                DBType dbtype = BP.Sys.SystemConfig.AppCenterDBType;

                if (dbtype == DBType.Oracle)
                {
                    DBAccess.RunSQL("ALTER TABLE  WF_EMP add startFlows_temp CLOB");
                    //将需要改成大字段的项内容copy到大字段中
                    DBAccess.RunSQL("UPDate WF_EMP set startFlows_temp=STARTFLOWS");
                    //删除原有字段
                    DBAccess.RunSQL("ALTER TABLE  WF_EMP drop column STARTFLOWS");
                    //将大字段名改成原字段名
                    DBAccess.RunSQL("ALTER TABLE  WF_EMP rename column startFlows_temp to STARTFLOWS");

                }
                if (dbtype == DBType.MySQL)
                    DBAccess.RunSQL("ALTER TABLE WF_Emp modify StartFlows longtext ");
                if (dbtype == DBType.MSSQL)
                {
                    DataTable dtYueSu = DBAccess.RunSQLReturnTable("SELECT b.name, a.name FName from sysobjects b join syscolumns a on b.id = a.cdefault where a.id = object_id('WF_Emp') and a.Name='StartFlows' ");
                    if (dtYueSu.Rows.Count != 0)
                        DBAccess.RunSQL(" ALTER TABLE WF_Emp drop  constraint " + dtYueSu.Rows[0][0]);

                    DBAccess.RunSQL(" ALTER TABLE WF_Emp ALTER column  StartFlows text");
                }

                if (dbtype == DBType.PostgreSQL)
                {
                    //  DBAccess.RunSQL(" ALTER TABLE WF_Emp ALTER column StartFlows type text");
                }


                #endregion 更新wf_emp 的字段类型.

                BP.Sys.FrmRB rb = new FrmRB();
                rb.CheckPhysicsTable();

                CC ccEn = new CC();
                ccEn.CheckPhysicsTable();

                BP.WF.Template.MapDtlExt dtlExt = new MapDtlExt();
                dtlExt.CheckPhysicsTable();

                MapData mapData = new MapData();
                mapData.CheckPhysicsTable();

                //删除枚举.
                DBAccess.RunSQL("DELETE FROM Sys_Enum WHERE EnumKey IN ('SelectorModel','CtrlWayAth')");

                //SysEnum se = new SysEnum("FrmType", 1);//NOTE:此处升级时报错，2017-06-13，liuxc

                //2017.5.19 打印模板字段修复
                BP.WF.Template.BillTemplate bt = new BillTemplate();
                bt.CheckPhysicsTable();
                if (DBAccess.IsExitsTableCol("WF_BillTemplate", "url") == true)
                    DBAccess.RunSQL("UPDATE WF_BillTemplate SET TempFilePath = Url WHERE TempFilePath IS null");

                //规范升级根目录.
                DataTable dttree = DBAccess.RunSQLReturnTable("SELECT No FROM Sys_FormTree WHERE ParentNo='-1' ");
                if (dttree.Rows.Count == 1)
                {
                    DBAccess.RunSQL("UPDATE Sys_FormTree SET ParentNo='1' WHERE ParentNo='0' ");
                    DBAccess.RunSQL("UPDATE Sys_FormTree SET No='1' WHERE No='0'  ");
                    DBAccess.RunSQL("UPDATE Sys_FormTree SET ParentNo='0' WHERE No='1'");
                }

                MapAttr myattr = new MapAttr();
                myattr.CheckPhysicsTable();

                //删除垃圾数据.
                BP.Sys.MapExt.DeleteDB();

                //升级傻瓜表单.
                MapFrmFool mff = new MapFrmFool();
                mff.CheckPhysicsTable();

                #region 表单方案中的不可编辑, 旧版本如果包含了这个列.
                if (BP.DA.DBAccess.IsExitsTableCol("WF_FrmNode", "IsEdit") == true)
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
                BP.WF.GenerWorkFlow gwf = new GenerWorkFlow();
                gwf.CheckPhysicsTable();
                sql = "SELECT WorkID,RDT FROM WF_GenerWorkFlow WHERE WeekNum=0 or WeekNum is null ";
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    sql = "UPDATE WF_GenerWorkFlow SET WeekNum=" + BP.DA.DataType.CurrentWeekGetWeekByDay(dr[1].ToString().Replace("+", " ")) + " WHERE WorkID=" + dr[0].ToString();
                    BP.DA.DBAccess.RunSQL(sql);
                }

                //查询.
                BP.WF.Data.CH ch = new CH();
                ch.CheckPhysicsTable();

                sql = "SELECT MyPK,DTFrom FROM WF_CH WHERE WeekNum=0 or WeekNum is null ";
                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    sql = "UPDATE WF_CH SET WeekNum=" + BP.DA.DataType.CurrentWeekGetWeekByDay(dr[1].ToString()) + " WHERE MyPK='" + dr[0].ToString() + "'";
                    BP.DA.DBAccess.RunSQL(sql);
                }
                #endregion  增加week字段.

                #region 检查数据源.
                SFDBSrc src = new SFDBSrc();
                src.No = "local";
                src.Name = "本机数据源(默认)";
                if (src.RetrieveFromDBSources() == 0)
                    src.Insert();
                #endregion 检查数据源.

                #region 20170613.增加审核组件配置项“是否显示退回的审核信息”对应字段 by:liuxianchen
                try
                {
                    if (BP.DA.DBAccess.IsExitsTableCol("WF_Node", "FWCIsShowReturnMsg") == false)
                    {
                        switch (src.HisDBType)
                        {
                            case DBType.MSSQL:
                                DBAccess.RunSQL("ALTER TABLE WF_Node ADD FWCIsShowReturnMsg INT NULL");
                                break;
                            case DBType.Oracle:
                            case DBType.Informix:
                            case DBType.PostgreSQL:
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
                                DBAccess.RunSQL("ALTER TABLE Sys_FrmRB ADD AtPara NVARCHAR2(1000) NULL");
                                break;
                            case DBType.PostgreSQL:
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
                BP.DA.DBAccess.RunSQL(sql);
                #endregion 其他.

                #region 升级统一规则.

                try
                {
                    string sqls = "";

                    if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                    {
                        sqls += "UPDATE Sys_MapExt SET MyPK= ExtType||'_'||FK_Mapdata||'_'||AttrOfOper WHERE ExtType='" + MapExtXmlList.TBFullCtrl + "'";
                        sqls += "@UPDATE Sys_MapExt SET MyPK= ExtType||'_'||FK_Mapdata||'_'||AttrOfOper WHERE ExtType='" + MapExtXmlList.PopVal + "'";
                        sqls += "@UPDATE Sys_MapExt SET MyPK= ExtType||'_'||FK_Mapdata||'_'||AttrOfOper WHERE ExtType='" + MapExtXmlList.DDLFullCtrl + "'";
                        sqls += "@UPDATE Sys_MapExt SET MyPK= ExtType||'_'||FK_Mapdata||'_'||AttrsOfActive WHERE ExtType='" + MapExtXmlList.ActiveDDL + "'";
                    }


                    if (SystemConfig.AppCenterDBType == DBType.MySQL)
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

                    BP.DA.DBAccess.RunSQLs(sqls);
                }
                catch (Exception ex)
                {
                    BP.DA.Log.DebugWriteError(ex.Message);
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
                BP.Sys.SFTable tab = new SFTable();
                tab.CheckPhysicsTable();
                #endregion

                #region 基础数据更新.
                Node wf_Node = new Node();
                wf_Node.CheckPhysicsTable();
                // 设置节点ICON.
                sql = "UPDATE WF_Node SET ICON='审核.png' WHERE ICON IS NULL";
                BP.DA.DBAccess.RunSQL(sql);

                BP.WF.Template.NodeSheet nodeSheet = new BP.WF.Template.NodeSheet();
                nodeSheet.CheckPhysicsTable();

                #endregion 基础数据更新.

                #region 把节点的toolbarExcel, word 信息放入mapdata
                BP.WF.Template.NodeSheets nss = new Template.NodeSheets();
                nss.RetrieveAll();
                foreach (BP.WF.Template.NodeSheet ns in nss)
                {
                    ToolbarExcel te = new ToolbarExcel();
                    te.No = "ND" + ns.NodeID;
                    te.RetrieveFromDBSources();
                }
                #endregion

                #region 升级SelectAccper
                Direction dir = new Direction();
                dir.CheckPhysicsTable();

                SelectAccper selectAccper = new SelectAccper();
                selectAccper.CheckPhysicsTable();
                #endregion

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

                BP.Sys.FrmAttachment myath = new FrmAttachment();
                myath.CheckPhysicsTable();

                FrmField ffs = new FrmField();
                ffs.CheckPhysicsTable();
                #endregion

                #region 执行sql．
                BP.DA.DBAccess.RunSQL("delete  from Sys_Enum WHERE EnumKey in ('BillFileType','EventDoType','FormType','BatchRole','StartGuideWay','NodeFormType')");
                DBAccess.RunSQL("UPDATE Sys_FrmSln SET FK_Flow =(SELECT FK_FLOW FROM WF_Node WHERE NODEID=Sys_FrmSln.FK_Node) WHERE FK_Flow IS NULL");

                if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                    DBAccess.RunSQL("UPDATE WF_FrmNode SET MyPK=FK_Frm+'_'+convert(varchar,FK_Node )+'_'+FK_Flow");

                if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                    DBAccess.RunSQL("UPDATE WF_FrmNode SET MyPK=FK_Frm||'_'||FK_Node||'_'||FK_Flow");

                if (SystemConfig.AppCenterDBType == DBType.MySQL)
                    DBAccess.RunSQL("UPDATE WF_FrmNode SET MyPK=CONCAT(FK_Frm,'_',FK_Node,'_',FK_Flow)");

                #endregion

                #region 检查必要的升级。
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

                #region 升级站内消息表 2013-10-20
                BP.WF.SMS sms = new SMS();
                sms.CheckPhysicsTable();
                #endregion

                #region 重新生成view WF_EmpWorks,  2013-08-06.

                if (DBAccess.IsExitsObject("WF_EmpWorks") == true)
                    BP.DA.DBAccess.RunSQL("DROP VIEW WF_EmpWorks");

                if (DBAccess.IsExitsObject("V_FlowStarter") == true)
                    BP.DA.DBAccess.RunSQL("DROP VIEW V_FlowStarter");

                if (DBAccess.IsExitsObject("V_FlowStarterBPM") == true)
                    BP.DA.DBAccess.RunSQL("DROP VIEW V_FlowStarterBPM");

                if (DBAccess.IsExitsObject("V_TOTALCH") == true)
                    BP.DA.DBAccess.RunSQL("DROP VIEW V_TOTALCH");

                if (DBAccess.IsExitsObject("V_TOTALCHYF") == true)
                    BP.DA.DBAccess.RunSQL("DROP VIEW V_TOTALCHYF");

                if (DBAccess.IsExitsObject("V_TotalCHWeek") == true)
                    BP.DA.DBAccess.RunSQL("DROP VIEW V_TotalCHWeek");

                if (DBAccess.IsExitsObject("V_WF_Delay") == true)
                    BP.DA.DBAccess.RunSQL("DROP VIEW V_WF_Delay");

                string sqlscript = "";
                //执行必须的sql.
                switch (BP.Sys.SystemConfig.AppCenterDBType)
                {
                    case DBType.Oracle:
                        sqlscript = BP.Sys.SystemConfig.PathOfData + "\\Install\\SQLScript\\InitView_Ora.sql";
                        break;
                    case DBType.MSSQL:
                    case DBType.Informix:
                        sqlscript = BP.Sys.SystemConfig.PathOfData + "\\Install\\SQLScript\\InitView_SQL.sql";
                        break;
                    case DBType.MySQL:
                        sqlscript = BP.Sys.SystemConfig.PathOfData + "\\Install\\SQLScript\\InitView_MySQL.sql";
                        break;
                    case DBType.PostgreSQL:
                        sqlscript = BP.Sys.SystemConfig.PathOfData + "\\Install\\SQLScript\\InitView_PostgreSQL.sql";
                        break;
                    default:
                        break;
                }

                BP.DA.DBAccess.RunSQLScript(sqlscript);
                #endregion

                #region 更新表单的边界.2014-10-18
                MapDatas mds = new MapDatas();
                mds.RetrieveAll();

                //  foreach (MapData md in mds)
                //    md.ResetMaxMinXY(); //更新边界.
                #endregion 更新表单的边界.

                #region 升级Img
                FrmImg img = new FrmImg();
                img.CheckPhysicsTable();

                BP.Sys.FrmUI.ExtImg myimg = new BP.Sys.FrmUI.ExtImg();
                myimg.CheckPhysicsTable();
                if (DBAccess.IsExitsTableCol("Sys_FrmImg", "SrcType") == true)
                {
                    DBAccess.RunSQL("UPDATE Sys_FrmImg SET ImgSrcType = SrcType WHERE ImgSrcType IS NULL");
                    DBAccess.RunSQL("UPDATE Sys_FrmImg SET ImgSrcType = 0 WHERE ImgSrcType IS NULL");
                }
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
                DBAccess.RunSQL("DELETE FROM Sys_Enum WHERE EnumKey IN ('DeliveryWay','RunModel','OutTimeDeal','FlowAppType')");
                #endregion 登陆更新错误

                #region 升级表单树
                // 首先检查是否升级过.
                sql = "SELECT * FROM Sys_FormTree WHERE No = '1'";
                DataTable formTree_dt = DBAccess.RunSQLReturnTable(sql);
                if (formTree_dt.Rows.Count == 0)
                {
                    /*没有升级过.增加根节点*/
                    SysFormTree formTree = new SysFormTree();
                    formTree.No = "1";
                    formTree.Name = "表单库";
                    formTree.ParentNo = "0";
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
                    sql = "UPDATE Sys_MapData SET FK_FormTree=FK_FrmSort WHERE FK_FrmSort <> '' AND FK_FrmSort IS not null";
                    DBAccess.RunSQL(sql);
                }
                #endregion

                #region 执行admin登陆. 2012-12-25 新版本.
                Emp emp = new Emp();
                emp.No = "admin";
                if (emp.RetrieveFromDBSources() == 1)
                {
                    BP.Web.WebUser.SignInOfGener(emp);
                }
                else
                {
                    emp.No = "admin";
                    emp.Name = "admin";
                    emp.FK_Dept = "01";
                    emp.Pass = "123";
                    emp.Insert();
                    BP.Web.WebUser.SignInOfGener(emp);
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

                #region 20161101.升级表单，增加图片附件必填验证 by:liuxianchen

                FrmImgAth imgAth = new FrmImgAth();
                imgAth.CheckPhysicsTable();

                sql = "UPDATE Sys_FrmImgAth SET IsRequired = 0 WHERE IsRequired IS NULL";
                BP.DA.DBAccess.RunSQL(sql);
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
                if (SystemConfig.IsEnablePasswordEncryption == true && BP.DA.DBAccess.IsView("Port_Emp", SystemConfig.AppCenterDBType) == false)
                {
                    BP.Port.Emps emps = new BP.Port.Emps();
                    emps.RetrieveAllFromDBSource();
                    foreach (Emp empEn in emps)
                    {
                        if (string.IsNullOrEmpty(empEn.Pass) || empEn.Pass.Length < 30)
                        {
                            empEn.Pass = BP.Tools.Cryptography.EncryptString(empEn.Pass);
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
                string err = "问题出处:" + ex.Message + "<hr>" + msg + "<br>详细信息:@" + ex.StackTrace + "<br>@<a href='../DBInstall.aspx' >点这里到系统升级界面。</a>";
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

            string sqlScript = SystemConfig.PathOfData + "\\UpdataCCFlowVer.sql";
            System.IO.FileInfo fi = new System.IO.FileInfo(sqlScript);
            string myVer = fi.LastWriteTime.ToString("MMddHHmmss");

            //判断是否可以执行，当文件发生变化后，才执行。
            if (currDBVer == "" || int.Parse(currDBVer) < int.Parse(myVer))
            {
                BP.DA.DBAccess.RunSQLScript(sqlScript);
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
                return BP.Sys.SystemConfig.GetValByKey("CCFlowAppPath", "/");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool IsEnableHuiQianList
        {
            get
            {
                if (SystemConfig.CustomerNo == "TianYe")
                    return true;

                return BP.Sys.SystemConfig.GetValByKeyBoolen("IsEnableHuiQianList", false);
            }
        }
        /// <summary>
        /// 检查是否可以安装系统
        /// </summary>
        /// <returns></returns>
        public static bool IsCanInstall()
        {
            try
            {
                string sql = "CREATE TABLE AA (XXX,DDD)";
                BP.DA.DBAccess.RunSQL(sql);

                sql = "CREATE TABLE AAVIEW AS SELECT * FROM AA";
                BP.DA.DBAccess.RunSQL(sql);

                sql = "DROP VIEW AAVIEW";
                BP.DA.DBAccess.RunSQL(sql);

                sql = "DROP TABLE AA";
                BP.DA.DBAccess.RunSQL(sql);
                return true;
            }
            catch
            {
                return false;
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

            #region 检查是否是空白的数据库。
            //if (BP.DA.DBAccess.IsExitsObject("WF_Emp")
            //     || BP.DA.DBAccess.IsExitsObject("WF_Flow")
            //     || BP.DA.DBAccess.IsExitsObject("Port_Emp")
            //    || BP.DA.DBAccess.IsExitsObject("CN_City"))
            //{
            //    throw new Exception("@当前的数据库好像是一个安装执行失败的数据库，里面包含了一些cc的表，所以您需要删除这个数据库然后执行重新安装。");
            //}
            #endregion 检查是否是空白的数据库。


            #region 首先创建Port类型的表, 让admin登录.

            FrmRB rb = new FrmRB();
            rb.CheckPhysicsTable();


            BP.Port.Emp portEmp = new BP.Port.Emp();
            portEmp.CheckPhysicsTable();

            BP.GPM.Emp myemp = new BP.GPM.Emp();
            myemp.CheckPhysicsTable();

            BP.GPM.Dept mydept = new BP.GPM.Dept();
            mydept.CheckPhysicsTable();

            BP.GPM.Station mySta = new BP.GPM.Station();
            mySta.CheckPhysicsTable();

            BP.GPM.StationType myStaType = new BP.GPM.StationType();
            myStaType.CheckPhysicsTable();

            BP.GPM.DeptEmp myde = new GPM.DeptEmp();
            myde.CheckPhysicsTable();

            BP.GPM.DeptEmpStation mydes = new GPM.DeptEmpStation();
            mydes.CheckPhysicsTable();

            BP.GPM.DeptStation mydeptSta = new GPM.DeptStation();
            mydeptSta.CheckPhysicsTable();

            BP.Sys.FrmRB myrb = new BP.Sys.FrmRB();
            myrb.CheckPhysicsTable();

            BP.WF.Port.WFEmp wfemp = new BP.WF.Port.WFEmp();
            wfemp.CheckPhysicsTable();

            if (BP.DA.DBAccess.IsExitsTableCol("WF_Emp", "StartFlows") == false)
            {
                string sql = "";
                //增加StartFlows这个字段
                switch (SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                        sql = "ALTER TABLE WF_Emp ADD StartFlows Text DEFAULT  NULL";
                        break;
                    case DBType.Oracle:
                        sql = "ALTER TABLE  WF_EMP add StartFlows BLOB";
                        break;
                    case DBType.MySQL:
                        sql = "ALTER TABLE WF_Emp ADD StartFlows TEXT COMMENT '可以发起的流程'";
                        break;
                    case DBType.Informix:
                        sql = "ALTER TABLE WF_Emp ADD StartFlows VARCHAR(4000) DEFAULT  NULL";
                        break;
                    case DBType.PostgreSQL:
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

            sqlscript = BP.Sys.SystemConfig.CCFlowAppPath + "\\WF\\Data\\Install\\SQLScript\\Port_Inc_CH_BPM.sql";
            BP.DA.DBAccess.RunSQLScript(sqlscript);

            BP.Port.Emp empAdmin = new Emp("admin");
            BP.Web.WebUser.SignInOfGener(empAdmin);
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

            CC cc = new CC();
            cc.CheckPhysicsTable();

            BP.WF.Template.FrmWorkCheck fwc = new FrmWorkCheck();
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
                if (clsName == null)
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
                switch (clsName)
                {
                    case "BP.WF.StartWork":
                    case "BP.WF.Work":
                    case "BP.WF.GEStartWork":
                    case "BP.En.GENoName":
                    case "BP.En.GETree":
                    case "BP.WF.Data.GERpt":
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
                case DBType.PostgreSQL:
                    sqlscript = BP.Sys.SystemConfig.CCFlowAppPath + "\\WF\\Data\\Install\\SQLScript\\InitView_PostgreSQL.sql";
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
                fs.No = "1";
                fs.ParentNo = "0";
                fs.Name = "流程树";
                fs.DirectInsert();
            }
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
                foreach (BP.Port.Emp emp in emps)
                {
                    for (int myIdx = 0; myIdx < 6; myIdx++)
                    {
                        string sql = "";
                        sql = "INSERT INTO Demo_Resume (OID,RefPK,NianYue,GongZuoDanWei,ZhengMingRen,BeiZhu,QT) ";
                        sql += "VALUES(" + DBAccess.GenerOID("Demo_Resume") + ",'" + emp.No + "','200" + myIdx + "-01','济南.驰骋" + myIdx + "公司','张三','表现良好','其他-" + myIdx + "无')";
                        DBAccess.RunSQL(sql);
                    }
                }

                DataTable dtStudent = BP.DA.DBAccess.RunSQLReturnTable("SELECT No FROM Demo_Student");
                foreach (DataRow dr in dtStudent.Rows)
                {
                    string no = dr[0].ToString();
                    for (int myIdx = 0; myIdx < 6; myIdx++)
                    {
                        string sql = "";
                        sql = "INSERT INTO Demo_Resume (OID,RefPK,NianYue,GongZuoDanWei,ZhengMingRen,BeiZhu,QT) ";
                        sql += "VALUES(" + DBAccess.GenerOID("Demo_Resume") + ",'" + no + "','200" + myIdx + "-01','济南.驰骋" + myIdx + "公司','张三','表现良好','其他-" + myIdx + "无')";
                        DBAccess.RunSQL(sql);
                    }
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

            #region 7, 装载 demo.flow
            if (isInstallFlowDemo == true)
            {
                BP.Port.Emp emp = new BP.Port.Emp("admin");
                BP.Web.WebUser.SignInOfGener(emp);
                BP.Sys.Glo.WriteLineInfo("开始装载模板...");
                string msg = "";

                //装载数据模版.
                BP.WF.DTS.LoadTemplete l = new BP.WF.DTS.LoadTemplete();
                msg = l.Do() as string;


                BP.Sys.Glo.WriteLineInfo("装载模板完成。开始修复视图...");


                BP.Sys.Glo.WriteLineInfo("视图修复完成。");
            }

            if (isInstallFlowDemo == false)
            {
                //创建一个空白的流程，不然，整个结构就出问题。
                FlowSorts fss = new FlowSorts();
                fss.RetrieveAll();
                fss.Delete();

                FlowSort fs = new FlowSort();
                fs.Name = "流程树";
                fs.No = "1";
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


                //创建一个空白的流程，不然，整个结构就出问题。
                BP.Sys.FrmTrees frmTrees = new FrmTrees();
                frmTrees.RetrieveAll();
                frmTrees.Delete();

                FrmTree ftree = new FrmTree();
                ftree.Name = "表单树";
                ftree.No = "1";
                ftree.ParentNo = "0";
                ftree.Insert();

                FrmTree subFrmTree = (FrmTree)ftree.DoCreateSubNode();
                subFrmTree.Name = "流程独立表单";
                subFrmTree.Update();

                subFrmTree = (FrmTree)ftree.DoCreateSubNode();
                subFrmTree.Name = "常用信息管理";
                subFrmTree.Update();

                subFrmTree = (FrmTree)ftree.DoCreateSubNode();
                subFrmTree.Name = "常用单据";
                subFrmTree.Update();

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
            BP.WF.DTS.GenerPinYinForEmp pinyin = new DTS.GenerPinYinForEmp();
            pinyin.Do();

            #region 执行补充的sql, 让外键的字段长度都设置成100.
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET maxlen=100 WHERE LGType=2 AND MaxLen<100");
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET maxlen=100 WHERE KeyOfEn='FK_Dept'");

            //Nodes nds = new Nodes();
            //nds.RetrieveAll();
            //foreach (Node nd in nds)
            //    nd.HisWork.CheckPhysicsTable();
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



        }
        /// <summary>
        /// 检查树结构是否符合需求
        /// </summary>
        /// <returns></returns>
        public static bool CheckTreeRoot()
        {
            // 流程树根节点校验
            string tmp = "SELECT Name FROM WF_FlowSort WHERE ParentNo='0'";
            tmp = DBAccess.RunSQLReturnString(tmp);
            if (string.IsNullOrEmpty(tmp))
            {
                tmp = "INSERT INTO WF_FlowSort(No,Name,ParentNo,TreeNo,idx,IsDir) values('01','流程树',0,'',0,0)";
                DBAccess.RunSQLReturnString(tmp);
            }

            // 表单树根节点校验
            tmp = "SELECT Name FROM Sys_FormTree WHERE ParentNo = '0' ";
            tmp = DBAccess.RunSQLReturnString(tmp);
            if (string.IsNullOrEmpty(tmp))
            {
                tmp = "INSERT INTO Sys_FormTree(No,Name,ParentNo,Idx,IsDir) values('001','表单树',0,0,0)";
                DBAccess.RunSQLReturnString(tmp);
            }

            //检查组织解构是否正确.
            string sql = "SELECT * FROM Port_Dept WHERE ParentNo='0' ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
            {
                BP.Port.Dept rootDept = new BP.Port.Dept();
                rootDept.Name = "总部";
                rootDept.ParentNo = "0";
                try
                {
                    rootDept.Insert();
                }
                catch (Exception ex)
                {
                    BP.DA.Log.DefaultLogWriteLineWarning("@尝试向port_dept插入数据失败，应该是视图问题. 技术信息:" + ex.Message);
                }
                throw new Exception("@没有找到部门树为0个根节点, 有可能是因为您在集成cc的时候，没有遵守cc的规则，部门树的根节点必须是ParentNo=0。");
            }

            if (BP.WF.Glo.OSModel == OSModel.OneOne)
            {
                try
                {
                    BP.Port.Dept dept = new BP.Port.Dept();
                    dept.Retrieve(BP.Port.DeptAttr.ParentNo, "0");
                }
                catch (Exception ex)
                {
                    throw new Exception("@cc的运行模式为OneOne @检查部门的时候错误:有可能是因为您在集成cc的时候，没有遵守cc的规则,Port_Dept列不符合要求，请仔细对比集成手册. 技术信息:" + ex.Message);
                }
            }

            if (BP.WF.Glo.OSModel == OSModel.OneMore)
            {
                try
                {
                    //  BP.GPM.Depts rootDepts = new BP.GPM.Depts("0");
                }
                catch (Exception ex)
                {
                    throw new Exception("@cc的运行模式为OneMore @检查部门的时候错误:有可能是因为您在集成cc的时候，没有遵守cc的规则,Port_Dept列不符合要求，请仔细对比集成手册. 技术信息:" + ex.Message);
                }
            }
            return true;
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
                str += "Rec,";
                str += "CDT,";
                return str;
                // return typeof(GERptAttr).GetFields().Select(o => o.Name).ToList();
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
            {
                //throw new Exception("@根据类名称获取流程事件实体实例出现错误:" + enName + ",没有找到该类的实体.");
                BP.DA.Log.DefaultLogWriteLineError("@根据类名称获取流程事件实体实例出现错误:" + enName + ",没有找到该类的实体.");
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

                string mark = "," + fee.FlowMark + ",";
                if (mark.Contains("," + flowMark + ",") == true)
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
            string doFunc, string WorkIDs)
        {
            if (doFunc == "SetParentFlow")
            {
                /* 如果需要设置子父流程信息.
                 * 应用于合并审批,当多个子流程合并审批,审批后发起一个父流程.
                 */

                GenerWorkFlow gwfParent = new GenerWorkFlow(workid);

                string[] workids = WorkIDs.Split(',');
                string okworkids = ""; //成功发送后的workids.
                GenerWorkFlow gwfSubFlow = new GenerWorkFlow();
                foreach (string id in workids)
                {
                    if (string.IsNullOrEmpty(id))
                        continue;

                    // 把数据copy到里面,让子流程也可以得到父流程的数据。
                    Int64 workidC = Int64.Parse(id);
                    gwfSubFlow.WorkID = workidC;
                    gwfSubFlow.RetrieveFromDBSources();

                    //设置当前流程的ID
                    BP.WF.Dev2Interface.SetParentInfo(gwfSubFlow.FK_Flow, workidC, gwfParent.WorkID);

                    // 是否可以执行？
                    if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(workidC, WebUser.No) == true)
                    {
                        //执行向下发送.
                        try
                        {
                            BP.WF.Dev2Interface.Node_SendWork(gwfSubFlow.FK_Flow, workidC);
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
                                BP.WF.Dev2Interface.Flow_DoUnSend(gwfSubFlow.FK_Flow, gwfSubFlow.WorkID);
                            }
                            #endregion 如果有一个发送失败，就撤销子流程与父流程.
                            throw new Exception("@在执行子流程(" + gwfSubFlow.Title + ")发送时出现如下错误:" + ex.Message);
                        }
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

                return "Port_DeptEmpStation";

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
        public static bool IsEnableTrackRec
        {
            get
            {
                string s = BP.Sys.SystemConfig.AppSettings["IsEnableTrackRec"];
                if (string.IsNullOrEmpty(s))
                    return false;
                if (s == "0")
                    return false;

                return true;
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
                backHtml += " <br> **** 流程结束,查看<a href='/WF/WFRpt.htm?WorkID=" + workid + "&FK_Flow=" + flowNo + "' target=_blank >流程轨迹</a> ====";
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
            return t.MyPK;
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
            exp = exp.Replace("@WebUser.FK_DeptNameOfFull", WebUser.FK_DeptNameOfFull);
            exp = exp.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);
            exp = exp.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);

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
        /// 前置导航导入表单数据
        /// </summary>
        /// <param name="WorkID"></param>
        /// <param name="FK_Flow"></param>
        /// <param name="FK_Node"></param>
        /// <param name="sKey">选中的No</param>
        public static DataTable StartGuidEnties(long WorkID, string FK_Flow, int FK_Node, string sKey)
        {
            Flow fl = new Flow(FK_Flow);
            switch (fl.StartGuideWay)
            {
                case StartGuideWay.SubFlowGuide:
                case StartGuideWay.BySQLOne:
                    string sql = "";
                    sql = fl.StartGuidePara3.Clone() as string;  //@李国文.
                    if (DataType.IsNullOrEmpty(sql) == false)
                    {
                        sql = sql.Replace("@Key", sKey);
                        sql = sql.Replace("@key", sKey);
                        sql = sql.Replace("~", "'");
                    }
                    else
                    {
                        sql = fl.StartGuidePara2.Clone() as string;
                    }

                    //sql = " SELECT * FROM (" + sql + ") T WHERE T.NO='" + sKey + "' ";

                    //替换变量
                    sql = sql.Replace("@WebUser.No", WebUser.No);
                    sql = sql.Replace("@WebUser.Name", WebUser.Name);
                    sql = sql.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);
                    sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                   

                    DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count == 0)
                        throw new Exception("err@没有找到那一行数据." + sql);

                    Hashtable ht = new Hashtable();
                    //转换成ht表
                    DataRow row = dt.Rows[0];
                    for (int i = 0; i < row.Table.Columns.Count; i++)
                    {
                        switch (row.Table.Columns[i].ColumnName.ToLower())
                        {
                            //去除关键字
                            case "no":
                            case "name":
                            case "workid":
                            case "fk_flow":
                            case "fk_node":
                            case "fid":
                            case "oid":
                            case "mypk":
                            case "title":
                            case "pworkid":
                                break;
                            default:
                                if (ht.ContainsKey(row.Table.Columns[i].ColumnName) == true)
                                    ht[row.Table.Columns[i].ColumnName] = row[i];  //@李国文.
                                else
                                    ht.Add(row.Table.Columns[i].ColumnName, row[i]);
                                break;
                        }
                    }
                    //保存
                    BP.WF.Dev2Interface.Node_SaveWork(FK_Flow, FK_Node, WorkID, ht);
                    return dt;
                case StartGuideWay.SubFlowGuideEntity:
                case StartGuideWay.BySystemUrlOneEntity:
                    break;
                default:
                    break;
            }

            return null;

        }
        /// <summary>
        /// 执行PageLoad装载数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="en"></param>
        /// <param name="mattrs"></param>
        /// <param name="dtls"></param>
        /// <returns></returns>
        public static Entity DealPageLoadFull(Entity en, MapExt item, MapAttrs mattrs, MapDtls dtls, bool isSelf = false, int nodeID = 0, long workID = 0)
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

                            if (string.IsNullOrEmpty(en.GetValStringByKey(dc.ColumnName)) || en.GetValStringByKey(dc.ColumnName) == "0")
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
                //如果有数据，就不要填充了.



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
                    if (isSelf == true)
                    {
                        MapDtl mdtlSln = new MapDtl();
                        mdtlSln.No = dtl.No + "_" + nodeID;
                        int result = mdtlSln.RetrieveFromDBSources();
                        if (result != 0)
                        {
                            //dtl.No = mdtlSln.No;
                            dtl.DtlOpenType = mdtlSln.DtlOpenType;
                        }
                    }



                    GEDtls gedtls = null;

                    try
                    {
                        gedtls = new GEDtls(dtl.No);
                        if (dtl.DtlOpenType == DtlOpenType.ForFID)
                        {
                            if (gedtls.RetrieveByAttr(GEDtlAttr.RefPK, workID) > 0)
                                continue;
                        }
                        else
                        {
                            if (gedtls.RetrieveByAttr(GEDtlAttr.RefPK, en.PKVal) > 0)
                                continue;
                        }


                        //gedtls.Delete(GEDtlAttr.RefPK, en.PKVal);
                    }
                    catch (Exception ex)
                    {
                        (gedtls.GetNewEntity as GEDtl).CheckPhysicsTable();
                    }

                    dt = DBAccess.RunSQLReturnTable(sql.StartsWith(dtl.No + "=")
                                                       ? sql.Substring((dtl.No + "=").Length)
                                                       : sql);
                    foreach (DataRow dr in dt.Rows)
                    {
                        GEDtl gedtl = gedtls.GetNewEntity as GEDtl;
                        foreach (DataColumn dc in dt.Columns)
                        {
                            gedtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());
                        }

                        switch (dtl.DtlOpenType)
                        {
                            case DtlOpenType.ForEmp:  // 按人员来控制.
                                gedtl.RefPK = en.PKVal.ToString();
                                gedtl.FID = long.Parse(en.PKVal.ToString());
                                break;
                            case DtlOpenType.ForWorkID: // 按工作ID来控制
                                gedtl.RefPK = en.PKVal.ToString();
                                gedtl.FID = long.Parse(en.PKVal.ToString());
                                break;
                            case DtlOpenType.ForFID: // 按流程ID来控制.
                                gedtl.RefPK = workID.ToString();
                                gedtl.FID = long.Parse(en.PKVal.ToString());
                                break;
                        }
                        gedtl.RDT = DataType.CurrentDataTime;
                        gedtl.Rec = WebUser.No;
                        gedtl.Insert();
                    }
                }
            }
            return en;
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
            sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);

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
                            /*如果不包含指定的关键的key, 就到公共变量里去找. */
                            if (BP.WF.Glo.SendHTOfTemp.ContainsKey(key) == false)
                                throw new Exception("@判断条件时错误,请确认参数是否拼写错误,没有找到对应的表达式:" + exp + " Key=(" + key + ") oper=(" + oper + ")Val=(" + val + ")");
                            valPara = BP.WF.Glo.SendHTOfTemp[key].ToString().Trim();
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
                throw new Exception("计算参数的时候出现错误:"+ex.Message);
            }
        }
        /// <summary>
        /// 表达式替换
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string DealExp(string exp, Entity en)
        {
            //替换字符
            exp = exp.Replace("~", "'");

            if (exp.Contains("@") == false)
                return exp;

            //首先替换加; 的。
            exp = exp.Replace("@WebUser.No;", WebUser.No);
            exp = exp.Replace("@WebUser.Name;", WebUser.Name);
            exp = exp.Replace("@WebUser.FK_DeptName;", WebUser.FK_DeptName);
            exp = exp.Replace("@WebUser.FK_Dept;", WebUser.FK_Dept);
            

            // 替换没有 ; 的 .
            exp = exp.Replace("@WebUser.No", WebUser.No);
            exp = exp.Replace("@WebUser.Name", WebUser.Name);
            exp = exp.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);
            exp = exp.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);

            if (exp.Contains("@") == false)
                return exp;

            //增加对新规则的支持. @MyField; 格式.
            if (en != null)
            {
                Attrs attrs = en.EnMap.Attrs;
                Row row = en.Row;
                //特殊判断.
                if (row.ContainsKey("OID") == true)
                    exp = exp.Replace("@WorkID", row["OID"].ToString());

                if (exp.Contains("@") == false)
                    return exp;

                foreach (string key in row.Keys)
                {
                    //值为空或者null不替换
                    if (row[key] == null || row[key].Equals("") == true)
                        continue;

                    if (exp.Contains("@" + key))
                    {
                        Attr attr = attrs.GetAttrByKeyOfEn(key);
                        //是枚举或者外键替换成文本
                        if (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum
                            || attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                        {
                            exp = exp.Replace("@" + key, row[key+"Text"].ToString());
                        }
                        else
                        {
                            if(attr.MyDataType == DataType.AppString  && attr.UIContralType ==  UIContralType.DDL && attr.MyFieldType ==FieldType.Normal)
                                 exp = exp.Replace("@" + key, row[key+"T"].ToString());
                            else
                                exp = exp.Replace("@" + key, row[key].ToString());
;
                        }

                        
                    }

                    //不包含@则返回SQL语句
                    if (exp.Contains("@") == false)
                        return exp;
                }

            }
          
            if (exp.Contains("@") && SystemConfig.IsBSsystem == true)
            {
                /*如果是bs*/
                foreach (string key in System.Web.HttpContext.Current.Request.QueryString.AllKeys)
                {
                    if (string.IsNullOrEmpty(key))
                        continue;
                    exp = exp.Replace("@" + key, System.Web.HttpContext.Current.Request.QueryString[key]);
                }
                /*如果是bs*/
                foreach (string key in System.Web.HttpContext.Current.Request.Form.AllKeys)
                {
                    if (string.IsNullOrEmpty(key))
                        continue;
                    exp = exp.Replace("@" + key, System.Web.HttpContext.Current.Request.Form[key]);
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
        public static string DealExp(string exp, Entity en, string errInfo)
        {
            //替换字符
            exp = exp.Replace("~", "'");

            if (exp.Contains("@") == false)
                return exp;

            //首先替换加; 的。
            exp = exp.Replace("@WebUser.No;", WebUser.No);
            exp = exp.Replace("@WebUser.Name;", WebUser.Name);
            exp = exp.Replace("@WebUser.FK_DeptName;", WebUser.FK_DeptName);
            exp = exp.Replace("@WebUser.FK_Dept;", WebUser.FK_Dept);
            

            // 替换没有 ; 的 .
            exp = exp.Replace("@WebUser.No", WebUser.No);
            exp = exp.Replace("@WebUser.Name", WebUser.Name);
            exp = exp.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);
            exp = exp.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);

            if (exp.Contains("@") == false)
                return exp;

            //增加对新规则的支持. @MyField; 格式.
            if (en != null)
            {
                Row row = en.Row;
                //特殊判断.
                if (row.ContainsKey("OID") == true)
                    exp = exp.Replace("@WorkID", row["OID"].ToString());

                if (exp.Contains("@") == false)
                    return exp;

                foreach (string key in row.Keys)
                {
                    //值为空或者null不替换
                    if (row[key] == null || row[key].Equals("") == true)
                        continue;

                    if (exp.Contains("@" + key + ";"))
                        exp = exp.Replace("@" + key + ";", row[key].ToString());

                    //不包含@则返回SQL语句
                    if (exp.Contains("@") == false)
                        return exp;
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
                    return exp;
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
                foreach (string key in System.Web.HttpContext.Current.Request.QueryString.AllKeys)
                {
                    if (string.IsNullOrEmpty(key))
                        continue;
                    exp = exp.Replace("@" + key, System.Web.HttpContext.Current.Request.QueryString[key]);
                }
                /*如果是bs*/
                foreach (string key in System.Web.HttpContext.Current.Request.Form.AllKeys)
                {
                    if (string.IsNullOrEmpty(key))
                        continue;
                    exp = exp.Replace("@" + key, System.Web.HttpContext.Current.Request.Form[key]);
                }

            }

            exp = exp.Replace("~", "'");
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
            DataTable dt = BP.DA.DBLoad.ReadExcelFileToDataTable(xlsFile);
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
            DataTable dt = BP.DA.DBLoad.ReadExcelFileToDataTable(xlsFile);
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
                    emp.Email = new BP.GPM.Emp(WebUser.No).Email;
                    emp.Insert();
                    DBAccess.RunSQL(p);
                }
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
                if (value == null)
                    return;

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
                    _AttrsOfRpt.AddTBString(GERptAttr.FlowEnderRDT, null, "最后处理时间", true, false, 0, 10, 10);
                    _AttrsOfRpt.AddTBInt(GERptAttr.FlowEndNode, 0, "停留节点", true, false);
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
        /// 全局的安全验证码
        /// </summary>
        public static string GloSID
        {
            get
            {
                string s = BP.Sys.SystemConfig.AppSettings["GloSID"] as string;
                if (DataType.IsNullOrEmpty(s))
                    s = "sdfq2erre-2342-234sdf23423-323";
                return s;
            }
        }
        /// <summary>
        /// 是否启用检查用户的状态?
        /// 如果启用了:在MyFlow.htm中每次都会检查当前的用户状态是否被禁
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
            return "<img src='" + CCFlowAppPath + "DataUser/Siganture/" + userNo + ".jpg' title='" + userName + "' border=0 onerror=\"src='" + CCFlowAppPath + "DataUser/UserIcon/DefaultSmaller.png'\" />";
        }
        /// <summary>
        /// 产生用户小图片
        /// </summary>
        /// <returns></returns>
        public static string GenerUserImgSmallerHtml(string userNo, string userName)
        {
            return "<img src='" + CCFlowAppPath + "DataUser/UserIcon/" + userNo + "Smaller.png' border=0  style='height:15px;width:15px;padding-right:5px;vertical-align:middle;'  onerror=\"src='" + CCFlowAppPath + "DataUser/UserIcon/DefaultSmaller.png'\" />" + userName;
        }
        /// <summary>
        /// 产生用户大图片
        /// </summary>
        /// <returns></returns>
        public static string GenerUserImgHtml(string userNo, string userName)
        {
            return "<img src='" + CCFlowAppPath + "DataUser/UserIcon/" + userNo + ".png'  style='padding-right:5px;width:60px;height:80px;border:0px;text-align:middle' onerror=\"src='" + CCFlowAppPath + "DataUser/UserIcon/Default.png'\" /><br>" + userName;
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
        /// 处理人员显示格式
        /// <para>added by liuxc,2017-4-27</para>
        /// </summary>
        /// <param name="emps">人员字符串，类似“duqinglian,杜清莲;wangyihan,王一涵;”</param>
        /// <param name="idBefore">是否用户id在前面、用户name在后面</param>
        /// <returns></returns>
        public static string DealUserInfoShowModel(string emps, bool idBefore = true)
        {
            if (string.IsNullOrWhiteSpace(emps))
                return emps;

            bool haveKH = emps.StartsWith("(");

            if (haveKH)
                emps = emps.Replace("(", "").Replace(")", "");

            string[] es = emps.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string newEmps = haveKH ? "(" : string.Empty;
            string[] ess = null;

            switch (BP.WF.Glo.UserInfoShowModel)
            {
                case UserInfoShowModel.UserIDOnly:
                    foreach (string e in es)
                    {
                        ess = e.Split(',');

                        if (ess.Length == 1)
                        {
                            newEmps += ess[0] + ";";
                            continue;
                        }

                        newEmps += (idBefore ? ess[0] : ess[1]) + ";";
                    }

                    return haveKH ? (newEmps + ")") : newEmps;
                case UserInfoShowModel.UserNameOnly:
                    foreach (string e in es)
                    {
                        ess = e.Split(',');

                        if (ess.Length == 1)
                        {
                            newEmps += ess[0] + ";";
                            continue;
                        }

                        newEmps += (idBefore ? ess[1] : ess[0]) + ";";
                    }

                    return haveKH ? (newEmps + ")") : newEmps;
                default:
                    return emps;
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
        /// 运行模式
        /// </summary>
        public static OSModel OSModel
        {
            get
            {
                return OSModel.OneMore;

                var model = BP.Sys.SystemConfig.GetValByKeyInt("OSModel", -1);
                return OSModel.OneMore;

                // OSModel os = (OSModel)BP.Sys.SystemConfig.GetValByKeyInt("OSModel", 0);
                // return os;
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
                return BP.Sys.SystemConfig.GetValByKeyInt("AutoNodeDTSTimeSpanMinutes", 60);
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
                if (BP.Sys.SystemConfig.IsBSsystem)
                {
                    /* 如果是BS 就要求 路径.*/
                }

                string baseUrl = BP.Sys.SystemConfig.AppSettings["HostURL"];
                if (string.IsNullOrEmpty(baseUrl) == true)
                    baseUrl = "http://127.0.0.1/";

                if (baseUrl.Substring(baseUrl.Length - 1) != "/")
                    baseUrl = baseUrl + "/";
                return baseUrl;
            }
        }
        /// <summary>
        /// 移动端主机
        /// </summary>
        public static string MobileURL
        {
            get
            {
                if (BP.Sys.SystemConfig.IsBSsystem)
                {
                    /* 如果是BS 就要求 路径.*/
                }

                string baseUrl = BP.Sys.SystemConfig.AppSettings["BpmMobileAddress"];
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
            DateTime mydt = BP.DA.DataType.AddDays(specDT, day, tway);
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
            DateTime mydt = BP.DA.DataType.AddDays(specDT, day, tway);
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
            if (nd.IsStartNode || nd.HisCHWay == CHWay.None)
                return;

            //如果设置为0,则不考核.
            if (nd.TimeLimit == 0 && nd.TimeLimitHH == 0 && nd.TimeLimitMM == 0)
                return;

            if (dtNow == null)
                dtNow = DateTime.Now;

            #region 求参与人员 todoEmps ，应完成日期 sdt ，与工作派发日期 prvRDT.
            //参与人员.
            string todoEmps = "";
            string dbstr = SystemConfig.AppCenterDBVarStr;
            if (nd.IsEndNode == true && gwl == null)
            {
                /* 如果是最后一个节点，可以使用这样的方式来求人员信息 , */

                #region 求应完成日期，与参与的人集合.
                Paras ps = new Paras();
                switch (SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                        ps.SQL = "SELECT TOP 1 SDTOfNode, TodoEmps FROM WF_GenerWorkFlow  WHERE WorkID=" + dbstr + "WorkID ";
                        break;
                    case DBType.Oracle:
                        ps.SQL = "SELECT SDTOfNode, TodoEmps FROM WF_GenerWorkFlow  WHERE WorkID=" + dbstr + "WorkID  ";
                        break;
                    case DBType.MySQL:
                        ps.SQL = "SELECT SDTOfNode, TodoEmps FROM WF_GenerWorkFlow  WHERE WorkID=" + dbstr + "WorkID  ";
                        break;
                    case DBType.PostgreSQL:
                        ps.SQL = "SELECT SDTOfNode, TodoEmps FROM WF_GenerWorkFlow  WHERE WorkID=" + dbstr + "WorkID  ";
                        break;
                    default:
                        throw new Exception("err@没有判断的数据库类型.");
                }

                ps.Add("WorkID", workid);
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(ps);
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


            if (nd.IsEndNode == false)
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

            ch.FK_NY = dtNow.ToString("yyyy-MM");

            ch.DTFrom = prvRDT;  //任务下达时间.
            ch.DTTo = dtNow.ToString("yyyy-MM-dd HH:mm:ss"); //时间到.

            ch.SDT = sdt; //应该完成时间.

            ch.FK_Flow = nd.FK_Flow; //流程信息.
            ch.FK_FlowT = nd.FlowName;

            ch.FK_Node = nd.NodeID; //节点.
            ch.FK_NodeT = nd.Name;

            ch.FK_Dept = WebUser.FK_Dept; //部门.
            ch.FK_DeptT = WebUser.FK_DeptName;

            ch.FK_Emp = WebUser.No;//当事人.
            ch.FK_EmpT = WebUser.Name;

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
            ch.MyPK = nd.NodeID + "_" + workid + "_" + fid + "_" + WebUser.No;
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
            #endregion 求计算属性.

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
                    ch.MyPK = BP.DA.DBAccess.GenerGUID();
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
        /// 删除临时文件
        /// </summary>
        public static void DeleteTempFiles()
        {
            try
            {
                //删除目录.
                string temp = BP.Sys.SystemConfig.PathOfTemp;
                System.IO.Directory.Delete(temp, true);

                //创建目录.
                System.IO.Directory.CreateDirectory(temp);

                //删除pdf 目录.
                temp = BP.Sys.SystemConfig.PathOfDataUser + "InstancePacketOfData\\";
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
        public static BP.Sys.FrmAttachmentDBs GenerFrmAttachmentDBs(FrmAttachment athDesc, string pkval, string FK_FrmAttachment,
            Int64 workid = 0, Int64 fid = 0, Int64 pworkid = 0, bool isContantSelf = true)
        {

            BP.Sys.FrmAttachmentDBs dbs = new BP.Sys.FrmAttachmentDBs();
            if (athDesc.HisCtrlWay == AthCtrlWay.PWorkID)
            {
                string pWorkID = BP.DA.DBAccess.RunSQLReturnValInt("SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + pkval, 0).ToString();
                if (pWorkID == null || pWorkID == "0")
                    pWorkID = pkval;

                if (athDesc.AthUploadWay == AthUploadWay.Inherit)
                {
                    /* 继承模式 */
                    BP.En.QueryObject qo = new BP.En.QueryObject(dbs);

                    if (pWorkID.Equals(pkval) == true)
                    {
                        qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, pkval);
                    }
                    else
                    {
                        qo.AddWhereIn(FrmAttachmentDBAttr.RefPKVal, '(' + pWorkID + ',' + pkval + ')');
                        //qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, "=", pWorkID, "RefPKVal1");
                        //qo.addOr();
                        //qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, "=", pkval, "");
                    }
                    qo.addOrderBy("RDT");
                    qo.DoQuery();
                }

                if (athDesc.AthUploadWay == AthUploadWay.Interwork)
                {
                    /*共享模式*/
                    dbs.Retrieve(FrmAttachmentDBAttr.RefPKVal, pWorkID);
                }
                return dbs;
            }

            if (athDesc.HisCtrlWay == AthCtrlWay.WorkID)
            {
                /* 继承模式 */
                BP.En.QueryObject qo = new BP.En.QueryObject(dbs);
                qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, pkval);
                qo.addAnd();
                qo.AddWhere(FrmAttachmentDBAttr.NoOfObj, athDesc.NoOfObj);
                if (isContantSelf == false)
                {
                    qo.addAnd();
                    qo.AddWhere(FrmAttachmentDBAttr.Rec, "!=", WebUser.No);
                }
                qo.addOrderBy("RDT");
                qo.DoQuery();
                return dbs;
            }

            if (athDesc.HisCtrlWay == AthCtrlWay.FID)
            {
                /* 继承模式 */
                BP.En.QueryObject qo = new BP.En.QueryObject(dbs);
                qo.AddWhere(FrmAttachmentDBAttr.FK_FrmAttachment, athDesc.MyPK);
                qo.addAnd();
                qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, int.Parse(pkval));
                if (isContantSelf == false)
                {
                    qo.addAnd();
                    qo.AddWhere(FrmAttachmentDBAttr.Rec, "!=", WebUser.No);
                }
                qo.addOrderBy("RDT");
                qo.DoQuery();
                return dbs;
            }


            if (athDesc.HisCtrlWay == AthCtrlWay.MySelfOnly || athDesc.HisCtrlWay == AthCtrlWay.PK)
            {
                if (FK_FrmAttachment.Contains("AthMDtl"))
                {
                    /*如果是一个明细表的多附件，就直接按照传递过来的PK来查询.*/
                    BP.En.QueryObject qo = new BP.En.QueryObject(dbs);
                    qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, pkval);
                    qo.addAnd();
                    qo.AddWhere(FrmAttachmentDBAttr.FK_FrmAttachment, FK_FrmAttachment);

                    qo.DoQuery();
                }
                else
                {
                    BP.En.QueryObject qo = new BP.En.QueryObject(dbs);
                    qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, pkval);
                    qo.addAnd();
                    qo.AddWhere(FrmAttachmentDBAttr.FK_FrmAttachment, FK_FrmAttachment);
                    if (isContantSelf == false)
                    {
                        qo.addAnd();
                        qo.AddWhere(FrmAttachmentDBAttr.Rec, "!=", WebUser.No);
                    }
                    qo.addOrderBy("RDT");
                    qo.DoQuery();

                    //dbs.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment, FK_FrmAttachment,
                    //   FrmAttachmentDBAttr.RefPKVal, pkval, "RDT");
                }
                return dbs;
            }

            throw new Exception("@没有判断的权限控制模式:" + athDesc.HisCtrlWay);

            return dbs;
        }
        /// <summary>
        /// 获得一个表单的动态附件字段
        /// </summary>
        /// <param name="exts">扩展</param>
        /// <param name="nd">节点</param>
        /// <param name="en">实体</param>
        /// <param name="md">map</param>
        /// <param name="attrs">属性集合</param>
        /// <returns>附件的主键</returns>
        public static string GenerActiveAths(MapExts exts, Node nd, Entity en, MapData md, MapAttrs attrs)
        {
            string strs = "";
            foreach (MapExt me in exts)
            {
                if (me.ExtType != MapExtXmlList.SepcAthSepcUsers)
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
                    if (DataType.IsNullOrEmpty(flow.StartLimitPara))
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
                    if (DataType.IsNullOrEmpty(flow.StartLimitPara))
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
                    if (DataType.IsNullOrEmpty(flow.StartLimitPara))
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
                    if (DataType.IsNullOrEmpty(flow.StartLimitPara))
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
                    if (DataType.IsNullOrEmpty(flow.StartLimitPara))
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


            //为子流程的时候，该子流程只能被调用一次.
            if (role == StartLimitRole.OnlyOneSubFlow)
            {

                if (BP.Sys.SystemConfig.IsBSsystem == true)
                {

                    string pflowNo = BP.Sys.Glo.Request.QueryString["PFlowNo"];
                    string pworkid = BP.Sys.Glo.Request.QueryString["PWorkID"];

                    if (pworkid == null)
                        return true;

                    sql = "SELECT Starter, RDT FROM WF_GenerWorkFlow WHERE PWorkID=" + pworkid + " AND FK_Flow='" + flow.No + "'";
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count == 0 || dt.Rows.Count == 1)
                        return true;

                    //  string title = dt.Rows[0]["Title"].ToString();
                    string starter = dt.Rows[0]["Starter"].ToString();
                    string rdt = dt.Rows[0]["RDT"].ToString();
                    return false;
                    //throw new Exception(flow.StartLimitAlert + "@该子流程已经被[" + starter + "], 在[" + rdt + "]发起，系统只允许发起一次。");
                }
            }
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

            //为子流程的时候，该子流程只能被调用一次.
            if (role == StartLimitRole.OnlyOneSubFlow)
            {
                sql = "SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + wk.OID;
                string pWorkidStr = DBAccess.RunSQLReturnStringIsNull(sql, "0");
                if (pWorkidStr == "0")
                    return true;

                sql = "SELECT Starter, RDT FROM WF_GenerWorkFlow WHERE PWorkID=" + pWorkidStr + " AND FK_Flow='" + flow.No + "'";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0 || dt.Rows.Count == 1)
                    return true;

                //  string title = dt.Rows[0]["Title"].ToString();
                string starter = dt.Rows[0]["Starter"].ToString();
                string rdt = dt.Rows[0]["RDT"].ToString();

                throw new Exception(flow.StartLimitAlert + "@该子流程已经被[" + starter + "], 在[" + rdt + "]发起，系统只允许发起一次。");
            }

            return true;
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
            frms.Delete(FrmFieldAttr.FK_Node, currNodeID, FrmFieldAttr.FK_MapData, frmID);

            //查询出来,指定的权限方案.
            frms.Retrieve(FrmFieldAttr.FK_Node, fromNodeID, FrmFieldAttr.FK_MapData, frmID);

            //开始复制.
            foreach (FrmField item in frms)
            {
                item.MyPK = frmID + "_" + fk_flow + "_" + currNodeID + "_" + item.KeyOfEn;
                item.FK_Node = currNodeID;
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
                fa.MyPK = fa.FK_MapData + "_" + fa.NoOfObj + "_" + currNodeID;
                fa.FK_Node = currNodeID;
                fa.Insert();
            }

            #endregion

        }
        /// <summary>
        /// 产生消息,senderEmpNo是为了保证写入消息的唯一性，receiveid才是真正的接收者.
        /// 如果插入失败.
        /// </summary>
        /// <param name="fromEmpNo">发送人</param>
        /// <param name="now">发送时间</param>
        /// <param name="msg">消息内容</param>
        /// <param name="sendToEmpNo">接受人</param>
        public static void SendMessageToCCIM(string fromEmpNo, string sendToEmpNo, string msg, string now)
        {
            //周朋.
            return;  //暂停支持.

            if (fromEmpNo == null)
                fromEmpNo = "";

            if (DataType.IsNullOrEmpty(sendToEmpNo))
                return;

            // throw new Exception("@接受人不能为空");

            string dbStr = SystemConfig.AppCenterDBVarStr;
            //保存系统通知消息
            StringBuilder strHql1 = new StringBuilder();
            //加密处理
            msg = BP.Tools.SecurityDES.Encrypt(msg);

            Paras ps = new Paras();
            string sql = "INSERT INTO CCIM_RecordMsg (OID,SendID,MsgDateTime,MsgContent,ImageInfo,FontName,FontSize,FontBold,FontColor,InfoClass,GroupID,SendUserID) VALUES (";
            sql += dbStr + "OID,";
            sql += "'SYSTEM',";
            sql += dbStr + "MsgDateTime,";
            sql += dbStr + "MsgContent,";
            sql += dbStr + "ImageInfo,";
            sql += dbStr + "FontName,";
            sql += dbStr + "FontSize,";
            sql += dbStr + "FontBold,";
            sql += dbStr + "FontColor,";
            sql += dbStr + "InfoClass,";
            sql += dbStr + "GroupID,";
            sql += dbStr + "SendUserID)";
            ps.SQL = sql;

            Int64 messgeID = BP.DA.DBAccess.GenerOID("RecordMsgUser");

            ps.Add("OID", messgeID);
            ps.Add("MsgDateTime", now);
            ps.Add("MsgContent", msg);
            ps.Add("ImageInfo", "");
            ps.Add("FontName", "宋体");
            ps.Add("FontSize", 10);
            ps.Add("FontBold", 0);
            ps.Add("FontColor", -16777216);
            ps.Add("InfoClass", 15);
            ps.Add("GroupID", -1);
            ps.Add("SendUserID", fromEmpNo);
            BP.DA.DBAccess.RunSQL(ps);

            //保存消息发送对象,这个是消息的接收人表.
            ps = new Paras();
            ps.SQL = "INSERT INTO CCIM_RecordMsgUser (OID,MsgId,ReceiveID) VALUES ( ";
            ps.SQL += dbStr + "OID,";
            ps.SQL += dbStr + "MsgId,";
            ps.SQL += dbStr + "ReceiveID)";

            ps.Add("OID", messgeID);
            ps.Add("MsgId", messgeID);
            ps.Add("ReceiveID", sendToEmpNo);
            BP.DA.DBAccess.RunSQL(ps);
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
